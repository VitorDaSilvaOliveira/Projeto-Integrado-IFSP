using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Utils;
using JJMasterData.Core.Events.Abstractions;
using JJMasterData.Core.Events.Args;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Estoque.Infrastructure.Services;

public class MovimentacaoService(
    IComponentFactory componentFactory,
    EstoqueDbContext context,
    ILogger<MovimentacaoService> logger,
    AuditLogService auditLogService,
    SignInManager<ApplicationUser> signInManager) : IFormEventHandler
{
    public async Task<JJFormView> GetFormViewMovimentacaoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Movimentacao");
        formView.ShowTitle = true;

        formView.OnAfterInsertAsync += OnAfterInsertAsync;

        return formView;
    }

    public async ValueTask OnAfterInsertAsync(object sender, FormAfterActionEventArgs e)
    {
        var values = e.Values;

        var produtoId = Convert.ToInt32(values["Id_Produto"]);
        var quantidade = Convert.ToInt32(values["Quantidade"]);
        var tipoMovimentacao = (TipoMovimentacao)Convert.ToInt32(values["TipoMovimentacao"]);
        var userId = values["Id_User"]?.ToString();

        await RegistrarMovimentacaoAsync(produtoId, quantidade, tipoMovimentacao, userId);
    }

    private async Task RegistrarMovimentacaoAsync(int produtoId, int quantidade, TipoMovimentacao tipoMovimentacao,
        string? userId)
    {
        var produto = await context.Produtos
            .FirstOrDefaultAsync(p => p.IdProduto == produtoId);

        var user = signInManager.Context.User;
        var userName = user?.Identity?.Name ?? "Anônimo";

        if (produto == null)
            logger.LogError("Produto não encontrado.");

        produto.QuantidadeEstoque ??= 0;

        if (tipoMovimentacao == TipoMovimentacao.Saida)
        {
            produto.QuantidadeEstoque -= quantidade;

            await auditLogService.LogAsync(
                "Estoque",
                "Movimentação de Saída",
                $"Saída de {quantidade} unidades do produto '{produto.Nome}' (ID: {produto.IdProduto}). Novo estoque: {produto.QuantidadeEstoque}.",
                userId,
                userName
            );


            if (produto.QuantidadeEstoque < produto.EstoqueMinimo)
                await NotificarEstoqueBaixo(produto, userId);
        }
        else if (tipoMovimentacao == TipoMovimentacao.Entrada)
        {
            produto.QuantidadeEstoque += quantidade;

            await auditLogService.LogAsync(
                "Estoque",
                "Movimentação de Entrada",
                $"Entrada de {quantidade} unidades do produto '{produto.Nome}' (ID: {produto.IdProduto}). Novo estoque: {produto.QuantidadeEstoque}.",
                userId,
                userName
            );
        }
    }

    private async Task NotificarEstoqueBaixo(Produto produto, string? userId)
    {
        var notificacao = new Notificacao
        {
            Mensagem =
                $"⚠️ Estoque do produto '{produto.Nome}' abaixo do mínimo! ({produto.QuantidadeEstoque} unidades restantes)",
            Data = LocalTime.Now(),
            IdUser = userId
        };

        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();
    }
}