using StarBord.DTOS;
namespace StarBord.Services.IService
{
    public interface IReviewService
    {
        Task<GetReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto, CancellationToken cancellationToken);
        Task<IEnumerable<GetReviewDto>> GetReviewsByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken);
        Task<GetReviewDto?> GetReviewByIdAsync(Guid reviewId, CancellationToken cancellationToken);
        Task<GetReviewDto> UpdateReviewAsync(Guid reviewId, CreateReviewDto updateReviewDto, CancellationToken cancellationToken);
        Task<bool> DeleteReviewAsync    (Guid reviewId, CancellationToken cancellationToken);
    }
}
