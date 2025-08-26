using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETicaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaret.Data.Configurations
{
    internal class BrandConfigurations : IEntityTypeConfiguration<Brand> 
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar(100)")
                .HasMaxLength(100);
            builder.Property(x => x.Logo)
                .IsRequired(false)
                .HasColumnType("nvarchar(200)")
                .HasMaxLength(200);
            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasColumnType("nvarchar(500)")
                .HasMaxLength(500);
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(x => x.OrderNo)
                .IsRequired()
                .HasDefaultValue(1);
            builder.Property(x => x.CreateDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.UpdateDate)
                .IsRequired(false);
            builder.HasMany(x => x.Products)
                .WithOne(x => x.Brand)
                .HasForeignKey(x => x.BrandId); 
            builder.HasData(            new Brand
            {
                Id = 1,
                Name = "Brand A",
                Logo = "brand_a_logo.png",
                Description = "Description for Brand A",
                IsActive = true,
                OrderNo = 1,
                CreateDate = new DateTime(2024, 1, 1)
            },
            new Brand
            {
                Id = 2,
                Name = "Brand B",
                Logo = "brand_b_logo.png",
                Description = "Description for Brand B",
                IsActive = true,
                OrderNo = 2,
                CreateDate = new DateTime(2024, 1, 1)
            });


        }
    }
}
