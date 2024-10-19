using Final.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Final.Data.Configurations
{
    public class BasketGameConfiguration : IEntityTypeConfiguration<BasketGame>
    {
        public void Configure(EntityTypeBuilder<BasketGame> builder)
        {
            // Primary Key
            builder.HasKey(bg => bg.Id);

            // Configure relationship between BasketGame and Basket
            builder
                .HasOne(bg => bg.Basket)
                .WithMany(b => b.BasketGames)
                .HasForeignKey(bg => bg.BasketId)
                .OnDelete(DeleteBehavior.Cascade);  // Allow cascading deletes on Basket

            // Configure relationship between BasketGame and Game
            builder
                .HasOne(bg => bg.Game)
                .WithMany(g => g.BasketGames)
                .HasForeignKey(bg => bg.GameId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading deletes on Game

            // Configure relationship between BasketGame and Dlc
            builder
                .HasOne(bg => bg.Dlc)
                .WithMany(d => d.BasketGames)
                .HasForeignKey(bg => bg.DlcId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading deletes on Dlc

            // Additional configurations for other properties
            builder.Property(bg => bg.Quantity)
                .IsRequired();  // Ensure Quantity is required

            builder.Property(bg => bg.Sum)
                .IsRequired();  // Ensure Sum is required
        }
    }



}
