using MediatR;
using Microsoft.EntityFrameworkCore;
using StarBord.Data;
using StarBord.DTOS;

namespace StarBord.Application.Responses;

public record GetResponseByIdQuery(Guid ResponseId) : IRequest<GetResponseDto?>;

public class GetResponseByIdQueryHandler : IRequestHandler<GetResponseByIdQuery, GetResponseDto?>
{
    private readonly StarBordDbContext _context;

    public GetResponseByIdQueryHandler(StarBordDbContext context) => _context = context;

    public async Task<GetResponseDto?> Handle(GetResponseByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Responses
            .Where(r => r.Id == request.ResponseId)
            .Select(r => new GetResponseDto
            {
                Id = r.Id,
                ReviewId = r.ReviewId,
                ResponseText = r.ResponseText,
                ResponseAt = r.ResponseAt,
                RespondedBy = r.RespondedBy
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }
}