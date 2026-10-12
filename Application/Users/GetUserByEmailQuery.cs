using MediatR;
using StarBord.DTOS;
using StarBord.Data;
using Microsoft.EntityFrameworkCore;
namespace StarBord.Application.Users
{
    public record GetUserByEmailQuery (string Email) : IRequest<GetUserDto?>;

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, GetUserDto?>
    {
        private readonly StarBordDbContext _context;

        public GetUserByEmailQueryHandler(StarBordDbContext context) => _context = context;

        public async Task<GetUserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Where(u => u.Email == request.Email)
                .Select(u => new GetUserDto
                {
                    Email = u.Email,
                    Username = u.Username
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            return user;
        }
    }
}
