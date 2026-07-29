using MediatR;
using StarBord.DTOS;
using StarBord.Data;
using StarBord.Models;
namespace StarBord.Application.Businesses
{
    public record CreateBusinessCommand(Guid UserId, CreateBusinessDto Business) : IRequest<GetBusinessDto>;

    public class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, GetBusinessDto>
    {
        private readonly StarBordDbContext _context;

        public CreateBusinessCommandHandler(StarBordDbContext context) => _context = context;

        public async Task<GetBusinessDto> Handle(CreateBusinessCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Business;
            var business = new Business
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = dto.Name,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Businesses.AddAsync(business, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
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