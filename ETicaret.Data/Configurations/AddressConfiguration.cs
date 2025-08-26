using ETicaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);
            
            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(a => a.FullName)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(a => a.Phone)
                .IsRequired()
                .HasMaxLength(20);
            
            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(a => a.District)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(a => a.FullAddress)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(a => a.IsActive)
                .HasDefaultValue(true);
            
            builder.Property(a => a.CreateDate)
                .HasDefaultValueSql("GETUTCDATE()");
            
            // Foreign Key Relationship
            builder.HasOne(a => a.AppUser)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(a => a.AppUserId);
            builder.HasIndex(a => a.IsActive);
        }
    }
}
