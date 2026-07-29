using MediatR;
using StarBord.DTOS;
using StarBord.Data;
namespace StarBord.Application.Reviews
{
    public record UpdateReviewCommand(Guid ReviewId,CreateReviewDto Review) : IRequest<GetReviewDto?>;
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, GetReviewDto?>
    {
        private readonly StarBordDbContext _context;
        private readonly ILogger<UpdateReviewCommandHandler> _logger;

        public UpdateReviewCommandHandler(StarBordDbContext context, ILogger<UpdateReviewCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews.FindAsync(request.ReviewId, cancellationToken);
            if (review == null)
            {
                _logger.LogWarning("Review with ID {ReviewId} not found for update.", request.ReviewId);
                throw new KeyNotFoundException($"Review with ID {request.ReviewId} not found.");
            }

            var dto = request.Review;
            review.BusinessId = dto.BusinessId;
            review.Rating = dto.Rating;
            review.ReviewText = dto.ReviewText;
            review.ReviewDate = dto.ReviewDate;
            review.Platform = dto.Platform;
            review.ExternalReviewId = dto.ExternalReviewId;
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync(cancellationToken);

            return new GetReviewDto
            {
                Id = review.Id,
                BusinessId = review.BusinessId,
                Rating = review.Rating,
                ReviewText = review.ReviewText,
                ReviewDate = review.ReviewDate,
                Platform = review.Platform,
                ExternalReviewId = review.ExternalReviewId
            };
        }
    }
}
