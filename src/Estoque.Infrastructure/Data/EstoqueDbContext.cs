using Estoque.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Data;

public class EstoqueDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) {}
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
}