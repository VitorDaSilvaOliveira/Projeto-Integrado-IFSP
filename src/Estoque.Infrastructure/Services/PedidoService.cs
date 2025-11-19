using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Documents;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;

namespace Estoque.Infrastructure.Services;

public class PedidoService(
    IComponentFactory componentFactory,
    EstoqueDbContext context,
    ILogger<MovimentacaoService> logger,
    MovimentacaoService movimentacaoService,
    IWebHostEnvironment env)
{

    public async Task<JJFormView> GetFormViewPedidoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Pedido");
        formView.ShowTitle = true;

        return formView;
    }

    public async Task<JJFormView> GetFormViewReportPedidoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("RelatorioPedido");
        formView.ShowTitle = true;
        return formView;
    }

    public async Task ProcessOrder(int idPedido, string userId)
    {
        var pedido = await context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == idPedido);

        var itensPedido = await context.PedidosItens
            .Where(pi => pi.id_Pedido == idPedido)
            .ToListAsync();

        try
        {
            foreach (var item in itensPedido)
            {
                await movimentacaoService.RegistrarMovimentacaoAsync(
                    item.ProdutoId,
                    item.Quantidade,
                    TipoMovimentacao.Saida,
                    userId,
                    $"Gerado Pedido {pedido!.NumeroPedido}"
                );
            }
            pedido.ValorTotal = itensPedido.Sum(i => i.Quantidade * i.PrecoVenda);
            pedido!.Status = PedidoStatus.Realizado;

            await context.SaveChangesAsync();
        }

        catch (Exception ex)
        {
            logger.LogError($"Erro ao confirmar pedido {idPedido}: {ex.Message}");
            throw;
        }
    }
    
    public async Task<byte[]> ReportPedidoAsync(int pedidoId)
    {
        var pedido = await context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido is null)
            throw new Exception("Pedido não encontrado.");

        var document = new RelatorioPedidoDocument(pedido, pedido.Itens.ToList(), env);
        return document.GeneratePdf();
    }

    public async Task<object> DashboardPedidoAsync()
    {
        var hoje = DateTime.Today;

        var pedidos = context.Pedidos.AsQueryable();

        // KPI
        var totalPedidos = await pedidos.CountAsync();
        var valorTotal = await pedidos.SumAsync(p => (decimal?)p.ValorTotal) ?? 0;
        var pedidosHoje = await pedidos.CountAsync(p => p.DataPedido >= hoje);
        var ticketMedio = totalPedidos > 0 ? valorTotal / totalPedidos : 0;

        // Gráfico: Operação
        var porOperacao = await pedidos
            .Where(p => p.Operacao != null)
            .GroupBy(p => p.Operacao)
            .Select(g => new {
                Operacao = g.Key.ToString(),
                Total = g.Count()
            })
            .ToListAsync();

        // Gráfico: Status
        var porStatus = await pedidos
            .Where(p => p.Status != null)
            .GroupBy(p => p.Status)
            .Select(g => new {
                Status = g.Key.ToString(),
                Total = g.Count()
            })
            .ToListAsync();

        // Gráfico: Pedidos por dia
        var porDia = await pedidos
            .Where(p => p.DataPedido != null)
            .GroupBy(p => p.DataPedido!.Value.Date)
            .Select(g => new {
                Dia = g.Key,
                Total = g.Count()
            })
            .OrderBy(x => x.Dia)
            .ToListAsync();

        return new {
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
