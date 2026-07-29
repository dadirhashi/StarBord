using MediatR;
using StarBord.DTOS;
using StarBord.Data;
using Microsoft.EntityFrameworkCore;
namespace StarBord.Application.Reviews
{
    public record GetReviewsByBusinessIdQuery(Guid BusinessId) : IRequest<IEnumerable<GetReviewDto>>;
    public class GetReviewsByBusinessIdQueryHandler : IRequestHandler<GetReviewsByBusinessIdQuery, IEnumerable<GetReviewDto>>
    {
        private readonly StarBordDbContext _context;

        public GetReviewsByBusinessIdQueryHandler(StarBordDbContext context) => _context = context;

        public async Task<IEnumerable<GetReviewDto>> Handle(GetReviewsByBusinessIdQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BusinessId == request.BusinessId)
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
                .ToListAsync(cancellationToken);

            return reviews;
        }

    }
}
