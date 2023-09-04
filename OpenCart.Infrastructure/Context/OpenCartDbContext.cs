using Microsoft.EntityFrameworkCore;
using OpenCart.Models.Entities;

namespace OpenCart.Infrastructure.Context
{
    public class OpenCartDbContext : DbContext
    {
        public OpenCartDbContext(DbContextOptions<OpenCartDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CartItemImage> CartItemImages { get; set; }
    }
}
