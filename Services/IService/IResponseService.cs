using StarBord.DTOS;
namespace StarBord.Services.IService
{
    public interface IResponseService
    {
        Task<GetResponseDto> CreateResponseAsync(CreateResponseDto dto, Guid respondedBy, CancellationToken cancellationToken);
        Task<GetResponseDto?> GetResponseByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<GetResponseDto?> GetResponseByReviewIdAsync(Guid reviewId, CancellationToken cancellationToken);
        Task<GetResponseDto?> UpdateResponseAsync(Guid id, CreateResponseDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteResponseAsync(Guid id, CancellationToken cancellationToken);
    }
}
