
using Microsoft.EntityFrameworkCore;
using StarBord.Models;

namespace StarBord.Data
{
    public class StarBordDbContext : DbContext
    {
        public StarBordDbContext(DbContextOptions<StarBordDbContext> options) : base(options)
        {
        }

        public DbSet<Business> Businesses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<PlatformToken> PlatformTokens { get; set; }
    }
}
