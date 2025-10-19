using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using JJMasterData.Core.Events.Abstractions;
using JJMasterData.Core.Events.Args;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Estoque.Infrastructure.Services;

public class DevolucaoService(
IComponentFactory componentFactory,
EstoqueDbContext context,
ILogger<MovimentacaoService> logger,
AuditLogService auditLogService,
MovimentacaoService movimentacaoService,
SignInManager<ApplicationUser> signInManager) : IFormEventHandler
{
    public async Task<JJFormView> GetFormViewDevolucaoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Devolucao");
        formView.ShowTitle = true;
        
        formView.OnAfterInsertAsync += ProcessAfterInsertAsync;
        formView.OnAfterUpdateAsync += ProcessAfterInsertAsync;

        return formView;
    }

    public async ValueTask ProcessAfterInsertAsync(object sender, FormAfterActionEventArgs e)
    {
        var values = e.Values;

        Devolucao devolucao = await context.Devolucoes.FindAsync(Convert.ToInt32(values["idDevolucao"]));

        var itensDevolucao = context.DevolucoesItens
            .Where(di => di.IdDevolucao == devolucao.IdDevolucao)
            .ToList();

        foreach (var item in itensDevolucao)
        {
            var produto = await context.Produtos.FindAsync(item.IdProduto);
            if (produto == null || item.Devolvido == 1)
                continue;

            await movimentacaoService.RegistrarMovimentacaoAsync(
                produto.IdProduto,
                item.QuantidadeDevolvida,
                TipoMovimentacao.Devolucao,
                devolucao.IdUser,
                item.Motivo
            );

            item.Devolvido = 1;
        }

        await context.SaveChangesAsync();
    }
}