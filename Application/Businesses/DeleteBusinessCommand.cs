using MediatR;
using StarBord.Data;
using StarBord.DTOS;

namespace StarBord.Application.Businesses
{
    public record DeleteBusinessCommand(Guid BusinessId, Guid UserId) : IRequest<bool>;
    public class DeleteBusinessCommandHandler : IRequestHandler<DeleteBusinessCommand, bool>
    {
        private readonly StarBordDbContext _context;
        private readonly ILogger<DeleteBusinessCommandHandler> _logger;


        public DeleteBusinessCommandHandler(StarBordDbContext context, ILogger<DeleteBusinessCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle (DeleteBusinessCommand request, CancellationToken cancellationToken)
        {
            var business = await _context.Businesses.FindAsync(request.BusinessId, cancellationToken);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found.", request.BusinessId);
                return false;
            }

            if (business.UserId != request.UserId)
            {
                _logger.LogWarning("Unauthorized delete attempt on business {BusinessId} by user {UserId}. ", request.BusinessId, request.UserId);
                throw new UnauthorizedAccessException("You are Not authorized to delete this business");
               
            }
            _context.Businesses.Remove(business);
            await _context.SaveChangesAsync(cancellationToken);
            return true;


        }
    }
}
