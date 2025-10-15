using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using JJMasterData.Core.Events.Args;
using JJMasterData.Core.UI.Components;
<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Services;

public class PedidoService(IComponentFactory componentFactory, EstoqueDbContext context, MovimentacaoService movimentacaoService)
=======
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Estoque.Infrastructure.Services;

public class PedidoService(IComponentFactory componentFactory,
    EstoqueDbContext context,
    ILogger<MovimentacaoService> logger,
    AuditLogService auditLogService,
    MovimentacaoService movimentacaoService,
    SignInManager<ApplicationUser> signInManager)
>>>>>>> 4854e9b31d66da0cd3599e0e4ccfe8b538f69934
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

<<<<<<< HEAD
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

=======
    public async Task<string> ConfirmarPedidoEGerarMovimentacoes(int idPedido)
    {
        var pedido = await context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == idPedido);

        if (pedido == null)
            return $"Pedido com ID {idPedido} não encontrado.";

        var itensPedido = await context.PedidosItens
            .Where(pi => pi.id_Pedido == idPedido)
            .ToListAsync();

        if (!itensPedido.Any())
            return $"Nenhum item encontrado para o pedido {idPedido}.";

        try
        {
            foreach (var item in itensPedido)
            {
                await movimentacaoService.RegistrarMovimentacaoAsync(
                    item.id_Produto,
                    item.Quantidade,
                    TipoMovimentacao.Saida,
                    null
                );
            }

            pedido.Status = PedidoStatus.Realizado;

            await context.SaveChangesAsync();

            return $"Pedido {idPedido} confirmado com sucesso e movimentações geradas.";
        }
        catch (Exception ex)
        {
            // logar o erro (ex.Message)
            // ex: logger.LogError(ex, "Erro ao confirmar pedido {Id}", idPedido);
            return $"Erro ao confirmar pedido {idPedido}: {ex.Message}";
        }
    }


}
>>>>>>> 4854e9b31d66da0cd3599e0e4ccfe8b538f69934
