using Final.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Final.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            // Configure the relationships between Comment and User
            builder.HasOne(c => c.User)
                .WithMany(u => u.Comments) // If User has a collection of Comments
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationships between Comment and Game
            builder.HasOne(c => c.Game)
                .WithMany(g => g.Comments) // If Game has a collection of Comments
                .HasForeignKey(c => c.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Additional configurations if needed
            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(1000); // Example: Setting maximum length for Content
        }
    }
}
