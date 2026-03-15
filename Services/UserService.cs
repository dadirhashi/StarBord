using StarBord.Services.IService;
using StarBord.Models;
using System;
using Microsoft.EntityFrameworkCore;
using StarBord.Data;

namespace StarBord.Services
{
    public class UserService : IUserService
    {
        private readonly StarBordDbContext _context;

        public UserService(StarBordDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<User?> GetUserByIdAsync(Guid id,CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User> CreateUserAsync(User user,CancellationToken cancellationToken)
        {
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }
    }
}
