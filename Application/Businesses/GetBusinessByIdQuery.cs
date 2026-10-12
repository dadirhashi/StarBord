using MediatR;
using StarBord.DTOS;
using StarBord.Data;
namespace StarBord.Application.Businesses
{
    public record GetBusinessByIdQuery(Guid BusinessId) : IRequest<GetBusinessDto>;
    public class GetBusinessByIdQueryHandler : IRequestHandler<GetBusinessByIdQuery, GetBusinessDto>
    {
        private readonly StarBordDbContext _context;

        public GetBusinessByIdQueryHandler(StarBordDbContext context)
        {
            _context = context;
        }

        public async Task<GetBusinessDto> Handle(GetBusinessByIdQuery request, CancellationToken cancellationToken)
        {
            var business = await _context.Businesses.FindAsync(request.BusinessId, cancellationToken);
            if (business == null)
            {
                throw new KeyNotFoundException($"Business with ID {request.BusinessId} not found.");
            }
            return new GetBusinessDto
            {
                Id = business.Id,
                UserId = business.UserId,
                Name = business.Name,
                Address = business.Address,
                CreatedAt = business.CreatedAt
            };
        }
    }
}
