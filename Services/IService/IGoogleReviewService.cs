using StarBord.DTOS;

namespace StarBord.Services.IService
{
    public interface IGoogleReviewService
    {
        Task<IEnumerable<GetReviewDto>> FetchReviewAsync(Guid businessId,CancellationToken cancellationToken);

    }
}
