using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETicaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.Cryptography;

namespace ETicaret.Data.Configurations
{

    public class AppUserConfigurations : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);
            builder.Property(x => x.SurName)
                .IsRequired()
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);
            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("nvarchar(100)")
                .HasMaxLength(100);
            builder.Property(x => x.Phone)
                .IsRequired(false)
                .HasColumnType("nvarchar(15)")
                .HasMaxLength(15);
            builder.Property(x => x.Password)
                .IsRequired()
                .HasColumnType("nvarchar(100)")
                .HasMaxLength(100);
            builder.Property(x => x.UserName)
                .IsRequired(false)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);
            builder.HasData(new AppUser
            {
                Id = 1,
                Name = "Admin",
                SurName = "User",
                Email = "admin@gmail.com",
                Phone = "1234567890",
                Password = HashPassword("Admin1234"), 
                UserName = "admin",
                IsActive = true,
                IsAdmin = true,
                CreateDate = DateTime.UtcNow,
                UserGuid = Guid.NewGuid()
            });
        
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
