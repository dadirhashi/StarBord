using StarBord.DTOS;
namespace StarBord.Services.IService
{
    public interface IReviewService
    {
        Task<GetReviewDto> CreateReview (CreateReviewDto createReviewDto, CancellationToken cancellationToken);
        Task<IEnumerable<GetReviewDto>> GetReviewsByBusinessId(Guid businessId, CancellationToken cancellationToken);
        Task<GetReviewDto?> GetReviewById(Guid reviewId, CancellationToken cancellationToken);
        Task<GetReviewDto> UpdateReview(Guid reviewId, CreateReviewDto updateReviewDto, CancellationToken cancellationToken);
        Task<bool> DeleteReview(Guid reviewId, CancellationToken cancellationToken);
    }
}
