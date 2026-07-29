using MediatR;
using Microsoft.EntityFrameworkCore;
using StarBord.Data;
using StarBord.DTOS;

namespace StarBord.Application.Businesses
{
    public record GetAllBusinessByUserIdQuery (Guid UserId) : IRequest<List<GetBusinessDto>>;
    public class GetAllBusinessByUserIdQueryHandler : IRequestHandler<GetAllBusinessByUserIdQuery, List<GetBusinessDto>>
    {
        private readonly StarBordDbContext _context;    
        public GetAllBusinessByUserIdQueryHandler(StarBordDbContext context) => _context = context;

        public async Task<List<GetBusinessDto>> Handle (GetAllBusinessByUserIdQuery request, CancellationToken cancellationToken)
        {
            var businesses = await _context.Businesses
                .Where(b => b.UserId == request.UserId)
                .Select(b => new GetBusinessDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    Name = b.Name,
                    Address = b.Address,
                    CreatedAt = b.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return businesses;
        }

    }
}
