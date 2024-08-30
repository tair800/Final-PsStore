using Final.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Final.Data.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.HasOne(w => w.User)
                   .WithOne(u => u.Wishlist)
                   .HasForeignKey<Wishlist>(w => w.UserId);

            builder.HasMany(w => w.WishlistGames)
                   .WithOne(wg => wg.Wishlist)
                   .HasForeignKey(wg => wg.WishlistId);
        }
    }

}
