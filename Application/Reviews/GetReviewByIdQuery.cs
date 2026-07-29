using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using StarBord.Data;
using StarBord.DTOS;

namespace StarBord.Application.Reviews
{
    public record GetReviewByIdQuery (Guid ReviewId) : IRequest <GetReviewDto?>;
    public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, GetReviewDto?>
    {
        private readonly StarBordDbContext _context;
        
        public GetReviewByIdQueryHandler(StarBordDbContext context)
        {
            _context = context;
        }

        public async Task<GetReviewDto?> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews
                .Where(r => r.Id == request.ReviewId)
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
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            return review;
        }
    }
}
