using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Utils;
using JJMasterData.Core.UI.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Estoque.Infrastructure.Services;

public class PedidoService(
    IComponentFactory componentFactory,
    EstoqueDbContext context,
    ILogger<MovimentacaoService> logger,
    //  AuditLogService auditLogService,
    MovimentacaoService movimentacaoService)
// SignInManager<ApplicationUser> signInManager)
{
    public int? statusPreUpdate = null;
    public int? statusPostUpdate = null;

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
                    $"Gerado Pedido {pedido!.NumeroPedido}",
                    null
                );
            }

            pedido!.Status = PedidoStatus.Realizado;

            await context.SaveChangesAsync();
        }

        catch (Exception ex)
        {
            logger.LogError($"Erro ao confirmar pedido {idPedido}: {ex.Message}");
            throw;
        }
    }

    public async Task<(List<string> labels, List<int> vendas, List<int> trocas)> GetPedidosPorOperacaoAsync()
    {

        var hoje = LocalTime.Now();
        var primeiroMes = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(-3);

        var pedidos = await context.Pedidos
            .Where(p => p.DataPedido != null && p.DataPedido >= primeiroMes)
            .ToListAsync();

        var labels = new List<string>();
        var vendas = new List<int>();
        var trocas = new List<int>();


        for (var i = 0; i < 4; i++)
        {
            var mesRef = primeiroMes.AddMonths(i);
            var proxMes = mesRef.AddMonths(1);

            vendas.Add(pedidos
                .Where(p => p.Operacao == 1 &&
                            p.DataPedido >= mesRef &&
                            p.DataPedido < proxMes)
                .Count());

            trocas.Add(pedidos
                .Where(p => p.Operacao == 2 &&
                            p.DataPedido >= mesRef &&
                            p.DataPedido < proxMes)
                .Count());

            labels.Add(mesRef.ToString("MMM", new CultureInfo("pt-BR")));
        }

        return (labels, vendas, trocas);
    }

}
