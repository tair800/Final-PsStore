using Final.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Final.Data.Data
{
    public class FinalDbContext : IdentityDbContext<User>
    {
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketGame> BasketGames { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Dlc> Dlcs { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GamePlatform> GamePlatforms { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistGame> WishlistGames { get; set; }

        public FinalDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
