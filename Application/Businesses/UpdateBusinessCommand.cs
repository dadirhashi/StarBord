using MediatR;
using StarBord.DTOS;
using StarBord.Data;
namespace StarBord.Application.Businesses
{
    public record UpdateBusinessCommand(CreateBusinessDto Business, Guid BusinessId) : IRequest<GetBusinessDto>;
    public class UpdateBusinessCommandHandler : IRequestHandler<UpdateBusinessCommand, GetBusinessDto>
    {
        private readonly StarBordDbContext _context;    
        public UpdateBusinessCommandHandler(StarBordDbContext context) => _context = context;   

        public async Task<GetBusinessDto> Handle (UpdateBusinessCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Business;
            var business = await _context.Businesses.FindAsync( request.BusinessId, cancellationToken);
            if (business == null)
            {
                throw new Exception($"Business with ID {request.BusinessId} not found.");
            }
            business.Name = dto.Name;
            business.Address = dto.Address;
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
