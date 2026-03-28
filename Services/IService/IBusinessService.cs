using StarBord.DTOS;
using StarBord.Models;

namespace StarBord.Services.IService
{
    public interface IBusinessService
    {
        Task <IEnumerable<GetBusinessDto>> GetAllBusinessAsync(CancellationToken cancellationToken);
        Task <GetBusinessDto?> GetBusinessByIdAsync(Guid id, CancellationToken cancellationToken);
        Task <GetBusinessDto> CreateBusinessAsync(Guid id,CreateBusinessDto businessDto, CancellationToken cancellationToken);
        Task <bool> DeleteBusinessAsync (Guid id, Guid UserId, CancellationToken cancellationToken);
    }
}
