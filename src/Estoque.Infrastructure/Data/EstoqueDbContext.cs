using Estoque.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Data;

public class EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Produto> Produtos { get; set; }
}