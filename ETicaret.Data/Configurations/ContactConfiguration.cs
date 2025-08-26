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
    internal class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.Surname)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.Email)
                .IsRequired(false)
                .HasMaxLength(100);
            builder.Property(c => c.Phone)
                .IsRequired(false)
                .HasColumnType("varchar(15)")
                .HasMaxLength(100);
            builder.Property(c => c.Message)
                .IsRequired() 
                .HasMaxLength(100);
            builder.Property(c => c.CreateDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
        }
    }
}