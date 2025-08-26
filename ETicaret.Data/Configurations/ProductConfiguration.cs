using ETicaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Data.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar(200)")
                .HasMaxLength(200);
            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasColumnType("nvarchar(max)")
                .HasMaxLength(1000);
            builder.Property(x => x.Image)
                .IsRequired(false)
                .HasColumnType("nvarchar(500)")
                .HasMaxLength(500);
            builder.Property(x => x.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.Property(x => x.ProductCode)
                .IsRequired(false)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);
            builder.Property(x => x.Stock)
                .IsRequired();
            builder.Property(x => x.CategoryId)
                .IsRequired();
            builder.Property(x => x.BrandId)
                .IsRequired();
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(x => x.IsHome)
                .IsRequired()
                .HasDefaultValue(false);
            builder.Property(x => x.OrderNo)
                .IsRequired()
                .HasDefaultValue(1);
            builder.Property(x => x.CreateDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Brand)
                 .WithMany(x => x.Products)
                 .HasForeignKey(x => x.BrandId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Product
                {
                    Id = 1,
                    Name = "Ürün 1",
                    Description = "Açıklama 1",
                    Image = "urun1.jpg",
                    Price = 100.00m,
                    ProductCode = "P001",
                    Stock = 50,
                    CategoryId = 1,
                    BrandId = 1,
                    IsActive = true,
                    IsHome = true,
                    OrderNo = 1,
                    CreateDate = new DateTime(2024, 1, 1)
                },
                new Product
                {
                    Id = 2,
                    Name = "Ürün 2",
                    Description = "Açıklama 2",
                    Image = "urun2.jpg",
                    Price = 150.00m,
                    ProductCode = "P002",
                    Stock = 30,
                    CategoryId = 1,
                    BrandId = 1,
                    IsActive = true,
                    IsHome = false,
                    OrderNo = 2,
                    CreateDate = new DateTime(2024, 1, 1)
                }
            );
        }   
    }
}
