//using Final.Core.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace Final.Data.Configurations
//{
//    public class DlcConfiguration : IEntityTypeConfiguration<Dlc>
//    {
//        public void Configure(EntityTypeBuilder<Dlc> builder)
//        {
//            builder
//                 .HasMany(d => d.BasketGames) // Assuming Dlc has a collection of BasketGames
//                 .WithOne(bg => bg.Dlc)        // BasketGame has a navigation property Dlc
//                 .HasForeignKey(bg => bg.DlcId)
//                 .OnDelete(DeleteBehavior.Restrict);

//        }
//    }

//}
