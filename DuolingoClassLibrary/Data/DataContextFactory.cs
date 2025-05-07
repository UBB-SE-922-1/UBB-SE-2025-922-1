using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace DuolingoClassLibrary.Data
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            // Get the project directory (two levels up from the Data directory)
            var projectDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\.."));

            // Build configuration
            var configuration = new ConfigurationBuilder()
           .SetBasePath(Path.Combine(Path.Combine(projectDir, "UBB-SE-2025-922-1"), "DuolingoClassLibrary"))
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();

            // Get connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
            }

            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new DataContext(optionsBuilder.Options);
        }
    }
}