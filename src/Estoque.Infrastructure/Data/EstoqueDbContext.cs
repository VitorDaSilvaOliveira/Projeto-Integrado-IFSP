using Estoque.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Estoque.Infrastructure.Data;

public class EstoqueDbContext : IdentityDbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; }
}

public class EstoqueDbContextFactory : IDesignTimeDbContextFactory<EstoqueDbContext>
{
    public EstoqueDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EstoqueDbContext>();

        optionsBuilder.UseSqlServer("Server=tcp:estoquesql.database.windows.net,1433;Initial Catalog=estoquesql;Persist Security Info=False;User ID=EstoqueProjIFSP;Password=SenhaEstoqueIFSP@2025;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        return new EstoqueDbContext(optionsBuilder.Options);
    }
}