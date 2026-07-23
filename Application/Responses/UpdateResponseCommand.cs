using MediatR;
using StarBord.Data;
using StarBord.DTOS;

namespace StarBord.Application.Responses
{
    public record UpdateResponseCommand(Guid ResponseId, Guid RespondedBy, CreateResponseDto Response) : IRequest<GetResponseDto?>;


    public class UpdateResponseCommandHandler : IRequestHandler<UpdateResponseCommand, GetResponseDto?>
    {
        private readonly StarBordDbContext _context;
        public UpdateResponseCommandHandler(StarBordDbContext context) => _context = context;

        public async Task<GetResponseDto?> Handle (UpdateResponseCommand request, CancellationToken cancellationToken)
        {
            var response = await _context.Responses.FindAsync(new object[] { request.ResponseId }, cancellationToken);
            if (response is null)
            {
               throw new KeyNotFoundException ($"Response with ID {request.ResponseId} not found.");
            }
            else if (response.RespondedBy != request.RespondedBy)
            {


                throw new UnauthorizedAccessException($"Response with ID {request.ResponseId} does not belong to User with ID {request.RespondedBy}.");
            }
            response.ResponseText = request.Response.ResponseText;
            await _context.SaveChangesAsync(cancellationToken);

            return new GetResponseDto
            {
                Id = response.Id,
                ReviewId = response.ReviewId,
                ResponseText = response.ResponseText,
                ResponseAt = response.ResponseAt,
                RespondedBy = response.RespondedBy
            };
        }

    }
}
