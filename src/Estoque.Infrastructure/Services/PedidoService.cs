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

    public async Task<IEnumerable<object>> ObterPedidosPorOperacaoAsync()
    {
        var agrupado = await context.Pedidos
            .GroupBy(p => p.Operacao)
            .Select(g => new
            {
                Operacao = g.Key.HasValue ? g.Key.ToString() : "Sem Operação",
                Total = g.Count()
            })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        return agrupado;
    }
}
