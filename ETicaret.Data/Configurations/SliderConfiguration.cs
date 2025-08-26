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
    class SliderConfiguration : IEntityTypeConfiguration<Slider>
	{
		public void Configure(EntityTypeBuilder<Slider> builder)
		{
			builder.Property(x => x.Title)
			   .IsRequired(false)
			   .HasColumnType("nvarchar(50)")
			   .HasMaxLength(50);
			builder.Property(x => x.Description)
				.IsRequired(false)
				.HasColumnType("nvarchar(200)")
				.HasMaxLength(200);
			builder.Property(x => x.Image)
				.IsRequired(false)
				.HasColumnType("nvarchar(200)")
				.HasMaxLength(500);
			builder.Property(x => x.Link)
				.IsRequired(false)
				.HasColumnType("nvarchar(1000)")
				.HasMaxLength(1000);
			builder.Property(x => x.IsActive)
				.IsRequired()
				.HasDefaultValue(true);
			builder.Property(x => x.CreateDate)
				.IsRequired()
				.HasDefaultValueSql("GETDATE()");
		}
    }
}
