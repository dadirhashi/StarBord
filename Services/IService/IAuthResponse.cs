using StarBord.DTOS;

namespace StarBord.Services.IService
{
    public interface IAuthResponse
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);
    }
}
