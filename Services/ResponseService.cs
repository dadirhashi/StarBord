using StarBord.Data;
using StarBord.DTOS;
using StarBord.Models;
using StarBord.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace StarBord.Services
{
    public class ResponseService : IResponseService
    {
        private readonly StarBordDbContext _context;
        private readonly ILogger<ResponseService> _logger;

        public ResponseService(StarBordDbContext context, ILogger<ResponseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetResponseDto> CreateResponseAsync(CreateResponseDto dto, Guid respondedBy, CancellationToken cancellationToken)
        {
            try
            {
                var response = new Response
                {
                    Id = Guid.NewGuid(),
                    ReviewId = dto.ReviewId,
                    ResponseText = dto.ResponseText,
                    ResponseAt = DateTime.UtcNow,
                    RespondedBy = respondedBy
                };

                await _context.Responses.AddAsync(response, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Response {ResponseId} created for review {ReviewId} by user {UserId}", response.Id, response.ReviewId, respondedBy);

                return new GetResponseDto
                {
                    Id = response.Id,
                    ReviewId = response.ReviewId,
                    ResponseText = response.ResponseText,
                    ResponseAt = response.ResponseAt,
                    RespondedBy = response.RespondedBy
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating response for review {ReviewId}", dto.ReviewId);
                throw;
            }
        }

        public async Task<GetResponseDto?> GetResponseByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _context.Responses
                    .Where(r => r.Id == id)
                    .Select(r => new GetResponseDto
                    {
                        Id = r.Id,
                        ReviewId = r.ReviewId,
                        ResponseText = r.ResponseText,
                        ResponseAt = r.ResponseAt,
                        RespondedBy = r.RespondedBy
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving response {ResponseId}", id);
                throw;
            }
        }

        public async Task<GetResponseDto?> GetResponseByReviewIdAsync(Guid reviewId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _context.Responses
                    .Where(r => r.ReviewId == reviewId)
                    .Select(r => new GetResponseDto
                    {
                        Id = r.Id,
                        ReviewId = r.ReviewId,
                        ResponseText = r.ResponseText,
                        ResponseAt = r.ResponseAt,
                        RespondedBy = r.RespondedBy
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving response for review {ReviewId}", reviewId);
                throw;
            }
        }

        public async Task<GetResponseDto?> UpdateResponseAsync(Guid id, CreateResponseDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _context.Responses.FindAsync(new object[] { id }, cancellationToken);
                if (response == null) return null;

                response.ResponseText = dto.ResponseText;
                response.ResponseAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Response {ResponseId} updated", id);

                return new GetResponseDto
                {
                    Id = response.Id,
                    ReviewId = response.ReviewId,
                    ResponseText = response.ResponseText,
                    ResponseAt = response.ResponseAt,
                    RespondedBy = response.RespondedBy
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating response {ResponseId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteResponseAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _context.Responses.FindAsync(new object[] { id }, cancellationToken);
                if (response == null)
                {
                    _logger.LogError("Response with id {ResponseId} not found for deletion", id);
                    return false;
                }

                _context.Responses.Remove(response);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Response {ResponseId} deleted", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting response {ResponseId}", id);
                throw;
            }
        }
    }
}