using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Repository;

public class PedidoRepository(EstoqueDbContext context)
{
    public async Task<Pedido?> GetPedidoAsync(int pedidoId)
    {
        return await context.Pedidos
            .Include(x => x.Cliente)
            .Include(x => x.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(x => x.Id == pedidoId);
    }

    public async Task<List<PedidoItem>> GetItensAsync(int idPedido)
    {
        return await context.PedidosItens
            .Include(x => x.Produto)
            .Where(x => x.id_Pedido == idPedido)
            .ToListAsync();
    }
    
    public Task<int> SaveAsync()
    {
        return context.SaveChangesAsync();
    }
    
    public async Task<object> DashboardAsync()
    {
        var hoje = DateTime.Today;
        var pedidos = context.Pedidos.AsQueryable();

        var totalPedidos = await pedidos.CountAsync();
        var valorTotal = await pedidos.SumAsync(p => p.ValorTotal) ?? 0;
        var pedidosHoje = await pedidos.CountAsync(p => p.DataPedido >= hoje);
        var ticketMedio = totalPedidos > 0 ? valorTotal / totalPedidos : 0;

        var porOperacao = await pedidos
            .Where(p => p.Operacao != null)
            .GroupBy(p => p.Operacao)
            .Select(g => new
            {
                Operacao = g.Key.ToString(),
                Total = g.Count()
            })
            .ToListAsync();

        var porStatus = await pedidos
            .Where(p => p.Status != null)
            .GroupBy(p => p.Status)
            .Select(g => new
            {
                Status = g.Key.ToString(),
                Total = g.Count()
            })
            .ToListAsync();

        var porDia = await pedidos
            .Where(p => p.DataPedido != null)
            .GroupBy(p => p.DataPedido!.Value.Date)
            .Select(g => new
            {
                Dia = g.Key,
                Total = g.Count()
            })
            .OrderBy(x => x.Dia)
            .ToListAsync();

        return new
        {
            TotalPedidos = totalPedidos,
            ValorTotal = valorTotal,
            TicketMedio = ticketMedio,
            PedidosHoje = pedidosHoje,
            PedidosPorOperacao = porOperacao,
            PedidosPorStatus = porStatus,
            PedidosPorDia = porDia
        };
    }
}