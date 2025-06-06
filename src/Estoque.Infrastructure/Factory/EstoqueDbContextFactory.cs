using Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Estoque.Infrastructure.Factory;

public class EstoqueDbContextFactory : IDesignTimeDbContextFactory<EstoqueDbContext>
{
    public EstoqueDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Estoque.Web"))
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<EstoqueDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new EstoqueDbContext(optionsBuilder.Options);
    }
}