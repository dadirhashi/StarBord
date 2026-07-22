using MediatR;
using StarBord.Data;
using StarBord.DTOS;

namespace StarBord.Application.Responses
{
    public record RespondToReviewCommand (Guid ReviewId): IRequest<GetResponseDto>;
    public class RespondToReviewCommandHandler :IRequestHandler<RespondToReviewCommand,GetResponseDto >
    {
        private readonly StarBordDbContext _context;
        public RespondToReviewCommandHandler(StarBordDbContext context) => _context = context;

        public async Task<GetBusinessDto> Handle(RespondToReviewCommand requst, CreateBusinessDto business)
        {
            var respond = await _context.FindAsync<RespondToReviewCommand(requst, business);>
        }
    
    }
}
