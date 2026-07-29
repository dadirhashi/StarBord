using MediatR;
using StarBord.Data;
using StarBord.Models;

namespace StarBord.Application.Reviews
{
    public record DeleteReviewCommand(Guid ReviewId) : IRequest<bool>;

    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, bool>
    {
        private readonly StarBordDbContext _context;
        private readonly ILogger<DeleteReviewCommandHandler> _logger;

        public DeleteReviewCommandHandler (StarBordDbContext context, ILogger<DeleteReviewCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews.FindAsync(request.ReviewId);
            if (review == null)
            {
                _logger.LogWarning("Review with id {ReviewId} not found for deletion", request.ReviewId);
                return false;
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
