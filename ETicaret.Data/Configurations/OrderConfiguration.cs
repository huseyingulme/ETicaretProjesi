using ETicaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            
            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            builder.Property(o => o.ShippingCost)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            builder.Property(o => o.Notes)
                .HasMaxLength(500);
            
            builder.Property(o => o.OrderStatus)
                .HasConversion<int>();
            
            builder.Property(o => o.PaymentMethod)
                .HasConversion<int>();
            
            // Foreign Key Relationships
            builder.HasOne(o => o.AppUser)
                .WithMany()
                .HasForeignKey(o => o.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(o => o.OrderNumber).IsUnique();
            builder.HasIndex(o => o.AppUserId);
            builder.HasIndex(o => o.OrderStatus);
            builder.HasIndex(o => o.CreateDate);
        }
    }
}
