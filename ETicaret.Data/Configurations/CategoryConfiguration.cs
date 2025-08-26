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
    internal class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);
            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasColumnType("nvarchar(max)")
                .HasMaxLength(500);
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(x => x.OrderNo)
                .IsRequired()
                .HasDefaultValue(0);
            builder.Property(x => x.CreateDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            builder.HasMany(x => x.Products)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId);
            
            builder.HasData(
                new Category
                {
                    Id = 1,
                    Name = "Kategori 1",
                    Description = "Kategori 1 Açıklaması",
                    IsActive = true,
                    IsTopMenu = true,
                    OrderNo = 1,
                    CreateDate = new DateTime(2024, 1, 1)
                }
            );
        }
    }
}
