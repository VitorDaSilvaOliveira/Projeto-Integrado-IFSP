using Estoque.Domain.Enums;
using Estoque.Infrastructure.Documents;
using Estoque.Infrastructure.Repository;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;

namespace Estoque.Infrastructure.Services;

public class PedidoService(
    IComponentFactory componentFactory,
    PedidoRepository repository,
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
        var pedido = await repository.GetPedidoAsync(idPedido);
        var itens = await repository.GetItensAsync(idPedido);

        try
        {
            foreach (var item in itens)
            {
                await movimentacaoService.RegistrarMovimentacaoAsync(
                    item.ProdutoId,
                    item.Quantidade,
                    TipoMovimentacao.Saida,
                    userId,
                    $"Gerado Pedido {pedido?.NumeroPedido}"
                );
            }

            pedido?.ValorTotal = itens.Sum(i => i.Quantidade * i.PrecoVenda);
            pedido?.Status = PedidoStatus.Realizado;

            await repository.SaveAsync();
        }

        catch (Exception ex)
        {
            logger.LogError("Erro ao confirmar pedido {IdPedido}: {ExMessage}", idPedido, ex.Message);
            throw;
        }
    }

    public async Task<byte[]> ReportPedidoAsync(int pedidoId)
    {
        var pedido = await repository.GetPedidoAsync(pedidoId);

        if (pedido is null)
            throw new Exception("Pedido não encontrado.");

        var document = new RelatorioPedidoDocument(pedido, pedido.Itens.ToList(), env);
        return document.GeneratePdf();
    }

    public async Task<object> DashboardPedidoAsync()
    {
        return await repository.DashboardAsync();
    }
}