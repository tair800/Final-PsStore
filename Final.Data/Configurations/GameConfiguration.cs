using Final.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Final.Data.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            //builder.HasMany(g => g.GamePlatforms)
            //       .WithOne(gp => gp.Game)
            //       .HasForeignKey(gp => gp.GameId);

            //builder.HasMany(g => g.WishlistGames)
            //       .WithOne(wg => wg.Game)
            //       .HasForeignKey(wg => wg.GameId);

        }
    }

}
