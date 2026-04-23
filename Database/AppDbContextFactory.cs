using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PROG7311Part2.Database
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
