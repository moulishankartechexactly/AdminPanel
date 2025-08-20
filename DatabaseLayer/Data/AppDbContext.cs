using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Entity.Models;

namespace DatabaseLayer.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Domain sets
        public DbSet<Product> Products { get; set; } = default!;

        // Note: IdentityDbContext<ApplicationUser> already exposes
        // public DbSet<ApplicationUser> Users { get; set; }
        // so an extra DbSet<ApplicationUser> is not required here.
    }
}
