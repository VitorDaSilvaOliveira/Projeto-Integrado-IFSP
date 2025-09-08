using Estoque.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Data;

public class EstoqueDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) {}
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Movimentacao> Movimentacoes { get; set; }
    public DbSet<Notificacao> Notificacoes { get; set; }
    public DbSet<Fornecedor> Fornecedores { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItem> PedidosItens { get; set; }
    public DbSet<ProdutoFornecedor> ProdutoFornecedores { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configura chave composta
        modelBuilder.Entity<ProdutoFornecedor>()
            .HasKey(pf => new { pf.IdProduto, pf.IdFornecedor });

        modelBuilder.Entity<ProdutoFornecedor>()
            .HasOne(pf => pf.Produto)
            .WithMany(p => p.ProdutoFornecedores)
            .HasForeignKey(pf => pf.IdProduto);

        modelBuilder.Entity<ProdutoFornecedor>()
            .HasOne(pf => pf.Fornecedor)
            .WithMany(f => f.ProdutoFornecedores)
            .HasForeignKey(pf => pf.IdFornecedor);
    }
}