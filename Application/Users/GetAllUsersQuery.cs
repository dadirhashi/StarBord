using MediatR;
using StarBord.DTOS;
using StarBord.Data;
using Microsoft.EntityFrameworkCore;
namespace StarBord.Application.Users
{
    public record GetAllUsersQuery : IRequest<List<GetUserDto>>;

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<GetUserDto>>
    {
        private readonly StarBordDbContext _context;
        public GetAllUsersQueryHandler(StarBordDbContext context) => _context = context;

        public async Task<List<GetUserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Users
                .Select(u => new GetUserDto
                {
                    Username = u.Username,
                    Email = u.Email
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return users;
        }
    }
}
