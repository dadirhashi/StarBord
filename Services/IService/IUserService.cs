using StarBord.Models;
using StarBord.DTOS;

namespace StarBord.Services.IService
{
    public interface IUserService
    {
        Task<IEnumerable<GetUserDto>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<GetUserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken);
    }
}
