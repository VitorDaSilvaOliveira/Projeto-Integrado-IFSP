using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Repository;

public class ClienteRepository(EstoqueDbContext context)
{
    public async Task<object> DashboardClienteAsync()
    {
        var totalClientes = await context.Clientes.CountAsync();

        var ativos = await context.Clientes
            .CountAsync(c => c.Status == UserStatus.Ativo);

        var inativos = await context.Clientes
            .CountAsync(c => c.Status == UserStatus.Inativo);

        var pedidosPorCliente = await context.Pedidos
            .Include(p => p.Cliente)
            .GroupBy(p => new { p.ClienteId, p.Cliente.Nome })
            .Select(g => new {
                NomeCliente = g.Key.Nome,
                QuantidadePedidos = g.Count(),
                TotalGasto = g.Sum(x => x.ValorTotal ?? 0)
            })
            .OrderByDescending(x => x.QuantidadePedidos)
            .Take(10)
            .ToListAsync();

        return new {
            TotalClientes = totalClientes,
            Ativos = ativos,
            Inativos = inativos,
            TopClientes = pedidosPorCliente
        };
    }
}