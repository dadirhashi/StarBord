using MediatR;
using StarBord.Data;
using Microsoft.EntityFrameworkCore;

namespace StarBord.Application.Responses
{
    public record DeleteResponseCommand (Guid ResponseId) : IRequest<bool>;
    public class DeleteResponseCommandHandler : IRequestHandler<DeleteResponseCommand, bool>
    {
        private readonly StarBordDbContext _context;
        public DeleteResponseCommandHandler(StarBordDbContext context) => _context = context;

        public async Task<bool> Handle(DeleteResponseCommand request, CancellationToken cancellationToken)
        {
         var response = await _context.Responses.FirstOrDefaultAsync(r => r.Id == request.ResponseId, cancellationToken);
            if (response == null)
            {
                return false;
            }
            _context.Responses.Remove(response);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
