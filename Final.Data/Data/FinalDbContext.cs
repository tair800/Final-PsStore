using Final.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Final.Data.Data
{
    public class FinalDbContext : IdentityDbContext<User>
    {
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketGame> BasketGames { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Dlc> Dlcs { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistGame> WishlistGames { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentHistory> CommentHistories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Promo> Promos { get; set; }
        public DbSet<UserCard> UserCards { get; set; }

        public FinalDbContext(DbContextOptions<FinalDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinalDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

    }
}
