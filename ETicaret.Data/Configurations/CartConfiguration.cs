using ETicaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.SessionId)
                .IsRequired()
                .HasMaxLength(450);
            
            builder.Property(c => c.UserId)
                .IsRequired(false);
            
            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            builder.Property(c => c.CreateDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
            
            builder.Property(c => c.UpdateDate)
                .IsRequired(false);
            
            // Relationships
            builder.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(c => c.SessionId);
            builder.HasIndex(c => c.UserId);
            builder.HasIndex(c => new { c.SessionId, c.IsActive });
        }
    }
}
