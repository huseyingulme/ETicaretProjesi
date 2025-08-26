using ETicaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Data.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(ci => ci.Id);
            
            builder.Property(ci => ci.CartId)
                .IsRequired();
            
            builder.Property(ci => ci.ProductId)
                .IsRequired();
            
            builder.Property(ci => ci.Quantity)
                .IsRequired()
                .HasDefaultValue(1);
            
            builder.Property(ci => ci.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            
            builder.Property(ci => ci.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            builder.Property(ci => ci.CreateDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
            
            builder.Property(ci => ci.UpdateDate)
                .IsRequired(false);
            
            // Relationships
            builder.HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(ci => ci.CartId);
            builder.HasIndex(ci => ci.ProductId);
            builder.HasIndex(ci => new { ci.CartId, ci.ProductId, ci.IsActive });
        }
    }
}
