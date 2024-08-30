using Final.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Final.Data.Configurations
{
    public class DlcConfiguration : IEntityTypeConfiguration<Dlc>
    {
        public void Configure(EntityTypeBuilder<Dlc> builder)
        {
            builder.HasOne(d => d.Game)
                   .WithMany(g => g.Dlcs)
                   .HasForeignKey(d => d.GameId);
        }
    }

}
