using StarBord.Services.IService;
using StarBord.Data;
using StarBord.DTOS;

namespace StarBord.Services
{
    public class Businessservice : IBusinessService
    {
        public readonly StarBordDbContext _context; 
        
        public Businessservice(StarBordDbContext context)
        {
            _context = context;
        }

        public Task<List<GetBusinessDto>> GetAllBusinessAsync(CancellationToken cancellationToken)
        {
            try
            {
                var businesses = _context.Businesses.Select(b => new GetBusinessDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    Name = b.Name,
                    Address = b.Address,
                    CreatedAt = b.CreatedAt
                }).ToList();
                return Task.FromResult(businesses);
            }
            catch (Exception ex)
            {

                
                return Task.FromResult(new List<GetBusinessDto>());
            }
        }

    }
}
