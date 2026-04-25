using StarBord.Services.IService;
using StarBord.Models;
using System;
using Microsoft.EntityFrameworkCore;
using StarBord.Data;
using StarBord.DTOS;

namespace StarBord.Services
{
    public class UserService : IUserService
    {
        private readonly StarBordDbContext _context;

        public UserService(StarBordDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetUserDto>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _context.Users.AsNoTracking().ToListAsync(cancellationToken);
            return users.Select(u => new GetUserDto
            {
                Username = u.Username,
                Email = u.Email,
            });
        }

        public async Task<GetUserDto?> GetUserByEmailAsync(string email,CancellationToken cancellationToken)
        {
            var user = await _context.Users.AsNoTracking()
                .Where(u => u.Email == email )
                .Select(u => new GetUserDto
                {
                    Username = u.Username,
                    Email = u.Email,
                })
                .FirstOrDefaultAsync(cancellationToken);
            return user;


        }

        public async Task<GetUserDto> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return new GetUserDto
            {
                Username = user.Username,
                Email = user.Email
            };
        }
    }
}
