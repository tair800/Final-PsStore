using Final.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Final.Data.Configurations
{
    public class BasketConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.HasOne(b => b.User)
               .WithOne(u => u.Basket)
               .HasForeignKey<Basket>(b => b.UserId);

            builder.HasMany(b => b.BasketGames)
                   .WithOne(bg => bg.Basket)
                   .HasForeignKey(bg => bg.BasketId);
        }
    }
}
