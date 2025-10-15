using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using JJMasterData.Core.Events.Args;
using JJMasterData.Core.UI.Components;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Services;

public class PedidoService(IComponentFactory componentFactory, EstoqueDbContext context, MovimentacaoService movimentacaoService)
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

    public async Task<bool> FinalizarPedidoAsync(int pedidoId)
    {
        var pedido = await context.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(pi => pi.Produto)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido == null || pedido.Status != PedidoStatus.EmAndamento)
        {
            return false;
        }

        foreach (var item in pedido.Itens)
        {
            // Baixar estoque
            var sucessoMovimentacao = await movimentacaoService.RegistrarSaidaAsync(
                item.ProdutoId,
                item.Quantidade,
                "Saída de estoque por finalização de pedido",
                item.Id
            );

            if (!sucessoMovimentacao)
            {
                // Se a movimentação falhar, o pedido não pode ser finalizado
                return false;
            }
        }

        pedido.Status = PedidoStatus.Finalizado;
        await context.SaveChangesAsync();
        return true;
    }
}

