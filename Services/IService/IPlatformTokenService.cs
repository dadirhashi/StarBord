using StarBord.DTOS;

namespace StarBord.Services.IService
{
    public interface IPlatformTokenService
    {
        Task<GetPlatformTokenDto> CreatePlatformTokenAsync(CreatePlatformTokenDto dto, CancellationToken cancellationToken);
        Task<GetPlatformTokenDto?> GetPlatformTokenByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<GetPlatformTokenDto>> GetPlatformTokensByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken);
        Task<GetPlatformTokenDto?> UpdatePlatformTokenAsync(Guid id, CreatePlatformTokenDto dto, CancellationToken cancellationToken);
        Task<bool> DeletePlatformTokenAsync(Guid id, CancellationToken cancellationToken);
    }
}
