using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PB303Fashion.DataAccessLayer.Entities;

namespace PB303Fashion.DataAccessLayer
{
    public class AppDbContext : IdentityDbContext<AppUser,IdentityRole<int>,int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Footer> Footers { get; set; } = null!;
        public DbSet<TopTrending> TopTrendings { get; set; }=null!;
    }
}
