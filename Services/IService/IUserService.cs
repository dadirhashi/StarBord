using StarBord.Models;

namespace StarBord.Services.IService
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User> CreateUserAsync(User user, CancellationToken cancellationToken);
    }
}
