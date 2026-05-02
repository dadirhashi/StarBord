using StarBord.Services.IService;
using StarBord.Data;
using StarBord.DTOS;
using Microsoft.EntityFrameworkCore;
using StarBord.Models;


namespace StarBord.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly StarBordDbContext _context;

        public BusinessService(StarBordDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetBusinessDto>> GetAllBusinessAsync(
            Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Businesses
                .Where(b => b.UserId == userId)
                .Select(b => new GetBusinessDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    Name = b.Name,
                    Address = b.Address,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<GetBusinessDto?> GetBusinessByIdAsync(
            Guid id, CancellationToken cancellationToken)
        {
            return await _context.Businesses
                .Where(b => b.Id == id)
                .Select(b => new GetBusinessDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    Name = b.Name,
                    Address = b.Address,
                    CreatedAt = b.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<GetBusinessDto> CreateBusinessAsync(
            Guid userId, CreateBusinessDto cbusinessDto, CancellationToken cancellationToken)
        {
            var business = new Business
            {
                Id = Guid.NewGuid(),
                UserId = userId,
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

        public async Task<bool> DeleteBusinessAsync(
            Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var business = await _context.Businesses
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId, cancellationToken);

            if (business == null) return false;

            _context.Businesses.Remove(business);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}