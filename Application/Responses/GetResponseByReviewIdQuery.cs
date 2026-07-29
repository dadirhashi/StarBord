using MediatR;
using StarBord.DTOS;
using StarBord.Data;
using Microsoft.EntityFrameworkCore;
namespace StarBord.Application.Responses
{
    public record GetResponseByReviewIdQuery(Guid ReviewId) : IRequest<GetResponseDto?>;
    public class GetResponseByReviewIdQueryHandler : IRequestHandler<GetResponseByReviewIdQuery, GetResponseDto?>
    {
        private readonly StarBordDbContext _context;
        public GetResponseByReviewIdQueryHandler(StarBordDbContext context) => _context = context;

        public async Task<GetResponseDto?> Handle(GetResponseByReviewIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Responses
                .Where(r => r.ReviewId == request.ReviewId)
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
        }

    }
}
