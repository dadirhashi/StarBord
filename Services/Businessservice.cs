using StarBord.Services.IService;
using StarBord.Data;
using StarBord.DTOS;
using Microsoft.EntityFrameworkCore;
using StarBord.Models;


namespace StarBord.Services
{
    public class Businessservice : IBusinessService
    {
        private readonly StarBordDbContext _context;
        private readonly ILogger<Businessservice> _logger;

        public Businessservice(StarBordDbContext context, ILogger<Businessservice> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<GetBusinessDto>> GetAllBusinessAsync(Guid userId, CancellationToken cancellationToken)
        {
        var businesses = await _context.Businesses.Where(b => b.UserId == userId).Select(b => new GetBusinessDto
        {
            Id = b.Id,
            UserId = b.UserId,
            Name = b.Name,
            Address = b.Address,
            CreatedAt = b.CreatedAt
        }).ToListAsync(cancellationToken);
        return businesses;
        }

        public async Task<GetBusinessDto?> GetBusinessByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var business = await _context.Businesses.Where(b => b.Id == id).Select(b => new GetBusinessDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    Name = b.Name,
                    Address = b.Address,
                    CreatedAt = b.CreatedAt
                }).FirstOrDefaultAsync(cancellationToken);
                return business;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving business with ID: {id}");
                throw new Exception($"An error occurred while retrieving business with ID: {id}", ex);
            }
        }

        public async Task<GetBusinessDto> CreateBusinessAsync(Guid userId, CreateBusinessDto cbusinessDto, CancellationToken cancellationToken)
        {
            try
            {
                var business = new Business
                {
                   
                    
                        Id = Guid.NewGuid(),
                        UserId = userId, // ← from the JWT token, not the DTO
                        Name = cbusinessDto.Name,
                        Address = cbusinessDto.Address,
                        CreatedAt = DateTime.UtcNow
                    
                };
                _context.Businesses.Add(business);
                await _context.SaveChangesAsync(cancellationToken);
                return new GetBusinessDto
                {
                    Id = business.Id,
                    UserId = business.UserId,
                    Name = business.Name,
                    Address = business.Address,
                    CreatedAt = business.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new business.");
                throw new Exception("An error occurred while creating a new business.", ex);
            }
        }

        public async Task<bool> DeleteBusinessAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var business = await _context.Businesses.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId, cancellationToken);
                if (business == null)
                {
                    return false;
                }
                _context.Businesses.Remove(business);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting business with ID: {id}");
                throw new Exception($"An error occurred while deleting business with ID: {id}", ex);
            }



        }
    }
}
