using MediatR;
using StarBord.Data;
using StarBord.DTOS;
using StarBord.Models;  
using Microsoft.EntityFrameworkCore;
using StarBord.Services;
using StarBord.Application.Exceptions;

namespace StarBord.Application.Responses
{
    public record RespondToReviewCommand (Guid ReviewId, Guid RespondedBy, CreateResponseDto ResponseDto): IRequest<GetResponseDto>;
    public class RespondToReviewCommandHandler :IRequestHandler<RespondToReviewCommand, GetResponseDto>
    {
        private readonly StarBordDbContext _context;
        private readonly ILogger<RespondToReviewCommandHandler> _logger;
        private ITrustpilotService _trustpilotService;

        public RespondToReviewCommandHandler(StarBordDbContext context, ILogger<RespondToReviewCommandHandler> logger, ITrustpilotService trustpilotService)
        {
            _context = context;
            _logger = logger;
            _trustpilotService = trustpilotService;
        }

        public async Task<GetResponseDto> Handle(
            RespondToReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews.FindAsync(new object[] { request.ReviewId }, cancellationToken);
            if (review is null)
            {
                throw new KeyNotFoundException($"Review with ID {request.ReviewId} not found.");
            }
            var alreadyResponded = await _context.Responses.AnyAsync(r => r.ReviewId == request.ReviewId, cancellationToken);
            if (alreadyResponded)
            {
                throw new ConflictException($"Review with ID {request.ReviewId} has already been responded to.");
            }

            var response = new Response
            {
                Id = Guid.NewGuid(),
                ReviewId = request.ReviewId,
                ResponseText = request.ResponseDto.ResponseText,
                ResponseAt = DateTime.UtcNow,
                RespondedBy = request.RespondedBy
            };

            await _context.Responses.AddAsync(response, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(review.ExternalReviewId))
            {
                _logger.LogWarning($"Review with ID {request.ReviewId} does not have an ExternalReviewId. Skipping Trustpilot response.");
            }
            else
            {
                try
                {
                    await _trustpilotService.PostReplyAsync(review.BusinessId, review.ExternalReviewId, response.ResponseText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to post response to Trustpilot for Review ID {request.ReviewId}.");
                }


            }

            return new GetResponseDto
            {
                Id = response.Id,
                ReviewId = response.ReviewId,
                ResponseText = response.ResponseText,
                ResponseAt = response.ResponseAt,
                RespondedBy = response.RespondedBy
            };
        }
    
    }
}
