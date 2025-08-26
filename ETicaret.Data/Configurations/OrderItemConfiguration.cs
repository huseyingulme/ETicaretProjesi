using ETicaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);
            
            builder.Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            builder.Property(oi => oi.Quantity)
                .IsRequired();
            
            // Foreign Key Relationships
            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Indexes
            builder.HasIndex(oi => oi.OrderId);
            builder.HasIndex(oi => oi.ProductId);
        }
    }
}
