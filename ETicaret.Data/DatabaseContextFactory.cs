using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ETicaret.Data
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(@"Server=HUSEYINGULME\MSSQLSERVER01;Database=E-TicaretDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new DatabaseContext(optionsBuilder.Options, null!);
        }
    }
}
