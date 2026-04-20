using Microsoft.EntityFrameworkCore;
using StarBord.Data;
using StarBord.DTOS;
using StarBord.Models;
using StarBord.Services.IService;

namespace StarBord.Services
{
    public class MockGoogleReviewService : IGoogleReviewService
    {
        private readonly StarBordDbContext _context;
        private readonly ILogger<MockGoogleReviewService> _logger;

        public MockGoogleReviewService(StarBordDbContext context, ILogger<MockGoogleReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<GetReviewDto>> FetchReviewAsync(Guid businessId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching mock Google reviews for business {BusinessId}", businessId);

                var mockReviews = new List<Review>
                {
                    new Review
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = businessId,
                        ReviewText = "Absolutely fantastic service! The staff were friendly and professional. Will definitely be coming back.",
                        Rating = 5,
                        Platform = "Google",
                        ReviewDate = DateTime.UtcNow.AddDays(-2),
                        ExternalReviewId = "google_mock_001"
                    },
                    new Review
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = businessId,
                        ReviewText = "Good experience overall but the wait time was a bit long. Quality of work was great though.",
                        Rating = 4,
                        Platform = "Google",
                        ReviewDate = DateTime.UtcNow.AddDays(-5),
                        ExternalReviewId = "google_mock_002"
                    },
                    new Review
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = businessId,
                        ReviewText = "Average experience. Nothing special but nothing bad either. Pricing was fair.",
                        Rating = 3,
                        Platform = "Google",
                        ReviewDate = DateTime.UtcNow.AddDays(-10),
                        ExternalReviewId = "google_mock_003"
                    },
                    new Review
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = businessId,
                        ReviewText = "Very disappointed. Booked an appointment and had to wait 45 minutes past my scheduled time.",
                        Rating = 2,
                        Platform = "Google",
                        ReviewDate = DateTime.UtcNow.AddDays(-15),
                        ExternalReviewId = "google_mock_004"
                    },
                    new Review
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = businessId,
                        ReviewText = "Best in town! I've been a loyal customer for years and they never disappoint. Highly recommend to everyone.",
                        Rating = 5,
                        Platform = "Google",
                        ReviewDate = DateTime.UtcNow.AddDays(-20),
                        ExternalReviewId = "google_mock_005"
                    }
                };

                // Save to database just like real API would
                foreach (var review in mockReviews)
                {
                    var exists = await _context.Reviews
                        .AnyAsync(r => r.ExternalReviewId == review.ExternalReviewId, cancellationToken);

                    if (!exists)
                    {
                        await _context.Reviews.AddAsync(review, cancellationToken);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Saved {Count} mock Google reviews for business {BusinessId}", mockReviews.Count, businessId);

                return mockReviews.Select(r => new GetReviewDto
                {
                    Id = r.Id,
                    BusinessId = r.BusinessId,
                    ReviewText = r.ReviewText,
                    Rating = r.Rating,
                    Platform = r.Platform,
                    ReviewDate = r.ReviewDate,
                    ExternalReviewId = r.ExternalReviewId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching mock Google reviews for business {BusinessId}", businessId);
                throw;
            }
        }
    }
}