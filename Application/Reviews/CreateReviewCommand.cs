using MediatR;
using StarBord.Data;
using StarBord.DTOS;
using StarBord.Models;


namespace StarBord.Application.Reviews
{
    public record CreateReviewCommand(CreateReviewDto Review) : IRequest<GetReviewDto>;
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, GetReviewDto>
    {
        private readonly StarBordDbContext _context;

        public CreateReviewCommandHandler(StarBordDbContext context)
        {
            _context = context;
        }

        public async Task<GetReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Review;
            var review = new Review
            {
                Id = Guid.NewGuid(),
                BusinessId = dto.BusinessId,
                ReviewText = dto.ReviewText,
                Rating = dto.Rating,
                Platform = dto.Platform,
                ReviewDate = dto.ReviewDate,
                ExternalReviewId = dto.ExternalReviewId
            };
            await _context.Reviews.AddAsync(review, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new GetReviewDto
            {
                Id = review.Id,
                BusinessId = review.BusinessId,
                ReviewText = review.ReviewText,
                Rating = review.Rating,
                Platform = review.Platform,
                ReviewDate = review.ReviewDate,
                ExternalReviewId = review.ExternalReviewId
            };
        }
    }
}
