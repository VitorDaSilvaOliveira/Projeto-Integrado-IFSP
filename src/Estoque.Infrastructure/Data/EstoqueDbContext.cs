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
    public DbSet<Devolucao> Devolucoes { get; set; }
    public DbSet<DevolucaoItem> DevolucoesItens { get; set; }
    public DbSet<Movimentacao> Movimentacoes { get; set; }
    public DbSet<Notificacao> Notificacoes { get; set; }
    public DbSet<Fornecedor> Fornecedores { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItem> PedidosItens { get; set; }
    public DbSet<SolicitacaoDevolucao> SolicitacoesDevolucao { get; set; }
    public DbSet<RoleMenu> RoleMenus { get; set; }
    public DbSet<ProdutoLote> ProdutoLotes { get; set; }
    public DbSet<ProdutoSerie> ProdutoSeries { get; set; }
    public DbSet<MasterData> MasterData { get; set; }
    public DbSet<Evento> Eventos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<RoleMenu>()
            .HasKey(rm => new { rm.RoleId, rm.MenuId });
    }
}