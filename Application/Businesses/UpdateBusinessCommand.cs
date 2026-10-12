using MediatR;
using StarBord.DTOS;
using StarBord.Data;
namespace StarBord.Application.Businesses
{
    public record UpdateBusinessCommand(Guid BusinessId, Guid UserId, CreateBusinessDto Business) : IRequest<GetBusinessDto>;
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
                throw new KeyNotFoundException($"Business with ID {request.BusinessId} not found.");
            }
            if (business.UserId != request.UserId)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} is not authorized to update this business.");
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
