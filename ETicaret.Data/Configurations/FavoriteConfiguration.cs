using ETicaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Data.Configurations
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(f => f.Id);
            
            builder.Property(f => f.UserId)
                .IsRequired()
                .HasMaxLength(450); // Identity kullanıcı ID'si için yeterli
            
            builder.Property(f => f.ProductId)
                .IsRequired();
            
            builder.Property(f => f.CreateDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
            
            builder.Property(f => f.IsActive)
                .IsRequired()
                .HasDefaultValue(true)
                .ValueGeneratedNever(); // Değerin otomatik oluşturulmasını engelle
            
            // Relationship with Product
            builder.HasOne(f => f.Product)
                .WithMany()
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Index for better performance
            builder.HasIndex(f => new { f.UserId, f.ProductId });
            builder.HasIndex(f => new { f.UserId, f.IsActive });
            
            // Unique constraint - bir kullanıcı aynı ürünü sadece bir kez favoriye ekleyebilir
            builder.HasIndex(f => new { f.UserId, f.ProductId })
                .IsUnique();
        }
    }
}
