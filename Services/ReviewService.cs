using StarBord.Data;
using StarBord.DTOS;
using StarBord.Services.IService;
using Microsoft.EntityFrameworkCore;
using StarBord.Models;

namespace StarBord.Services
{
    // SUPERSEDED by Application/Reviews CQRS handlers. Scheduled for removal in step 5 (cleanup).
    // Kept temporarily as reference during CQRS pilot. Do not add new logic here.
    public class ReviewService : IReviewService
    {
        private readonly StarBordDbContext _context;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(StarBordDbContext context, ILogger<ReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto, CancellationToken cancellationToken)
        {
            var review = new Review
            {
                Id = Guid.NewGuid(),
                BusinessId = createReviewDto.BusinessId,
                ReviewText = createReviewDto.ReviewText,
                Rating = createReviewDto.Rating,
                Platform = createReviewDto.Platform,
                ReviewDate = createReviewDto.ReviewDate,
                ExternalReviewId = createReviewDto.ExternalReviewId
            };

            var newReview = await _context.Reviews.AddAsync(review, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new GetReviewDto
            {
                Id = review.Id,
                BusinessId = review.BusinessId,
                ReviewText = review.ReviewText,
                Rating = review.Rating,
                Platform = review.Platform,
                ReviewDate = review.ReviewDate,
                ExternalReviewId = review.ExternalReviewId
            };
        }

        public async Task<IEnumerable<GetReviewDto>> GetReviewsByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BusinessId == businessId)
                .Select(r => new GetReviewDto
                {
                    Id = r.Id,
                    BusinessId = r.BusinessId,
                    ReviewText = r.ReviewText,
                    Rating = r.Rating,
                    Platform = r.Platform,
                    ReviewDate = r.ReviewDate,
                    ExternalReviewId = r.ExternalReviewId
                })
                .AsNoTracking().ToListAsync(cancellationToken);
            return reviews;
        }

        public async Task<GetReviewDto?> GetReviewByIdAsync(Guid reviewId, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews
                .Where(r => r.Id == reviewId)
                .Select(r => new GetReviewDto
                {
                    Id = r.Id,
                    BusinessId = r.BusinessId,
                    ReviewText = r.ReviewText,
                    Rating = r.Rating,
                    Platform = r.Platform,
                    ReviewDate = r.ReviewDate,
                    ExternalReviewId = r.ExternalReviewId
                }).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            return review;

        }

        public async Task<GetReviewDto> UpdateReviewAsync(Guid reviewId, CreateReviewDto updateReviewDto, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews.FindAsync(new object[] { reviewId }, cancellationToken);
            if (review == null)
            {
                _logger.LogError($"Review with id {reviewId} not found for update");
                throw new KeyNotFoundException("Review not found");
            }
            review.BusinessId = updateReviewDto.BusinessId;
            review.ReviewText = updateReviewDto.ReviewText;
            review.Rating = updateReviewDto.Rating;
            review.Platform = updateReviewDto.Platform;
            review.ReviewDate = updateReviewDto.ReviewDate;
            review.ExternalReviewId = updateReviewDto.ExternalReviewId;
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync(cancellationToken);
            return new GetReviewDto
            {
                Id = review.Id,
                BusinessId = review.BusinessId,
                ReviewText = review.ReviewText,
                Rating = review.Rating,
                Platform = review.Platform,
                ReviewDate = review.ReviewDate,
                ExternalReviewId = review.ExternalReviewId
            };
        }

        public async Task<bool> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews.FindAsync(new object[] { reviewId }, cancellationToken);
            if (review == null)
            {
                _logger.LogError("Review with id {ReviewId} not found for update", reviewId);
                return false;
            }
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);
            return true;

        }
    }
}