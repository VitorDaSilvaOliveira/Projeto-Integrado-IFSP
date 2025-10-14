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

        formView.OnAfterInsertAsync += GetValuesAsync;
        formView.OnAfterUpdateAsync += GetValuesAsync;

        return formView;
    }

    public async ValueTask GetValuesAsync(object sender, FormAfterActionEventArgs e)
    {
        var values = e.Values;

        int quantidade = 0;
        var produtoId = Convert.ToInt32(values["Id_Produto"]);
        var tipoMovimentacao = (TipoMovimentacao)Convert.ToInt32(values["TipoMovimentacao"]);
        var userId = values["Id_User"]?.ToString();

        await RegistrarMovimentacaoAsync(produtoId, quantidade, tipoMovimentacao, userId);
    }

    public async Task RegistrarMovimentacaoAsync(
        int produtoId,
        int quantidade,
        TipoMovimentacao tipoMovimentacao,
        string? userId)
    {
        var produto = await context.Produtos
            .Include(p => p.ProdutoLotes)
            .FirstOrDefaultAsync(p => p.IdProduto == produtoId);

        if (produto == null)
        {
            logger.LogError("Produto não encontrado.");
            return;
        }

        var user = signInManager.Context.User;
        var userName = user?.Identity?.Name ?? "Anônimo";

        if (tipoMovimentacao == TipoMovimentacao.Saida)
        {
            var qtdRestante = quantidade;

            foreach (var lote in produto.ProdutoLotes
                         .Where(l => l.QuantidadeDisponivel > 0)
                         .OrderBy(l => l.DataEntrada))
            {
                if (qtdRestante <= 0) break;

                var retirar = Math.Min(qtdRestante, lote.QuantidadeDisponivel);
                lote.QuantidadeDisponivel -= retirar;
                qtdRestante -= retirar;
            }

            if (qtdRestante > 0)
            {
                logger.LogWarning("Tentativa de saída maior que estoque disponível do produto {ProdutoId}", produtoId);
                throw new InvalidOperationException("Estoque insuficiente.");
            }

            var entradaResgistrada = new Movimentacao
            {
                IdProduto = produtoId,
                Quantidade = quantidade,
                TipoMovimentacao = TipoMovimentacao.Saida,
                DataMovimentacao = DateTime.UtcNow,
                IdUser = userId
            };

            context.Movimentacoes.Add(entradaResgistrada);

            await auditLogService.LogAsync(
                "Estoque",
                "Movimentação de Saída",
                $"Saída de {quantidade} unidades do produto '{produto.Nome}' (ID: {produto.IdProduto}).",
                userId,
                userName
            );

        }
        else if (tipoMovimentacao == TipoMovimentacao.Entrada)
        {
            var lote = new ProdutoLote
            {
                ProdutoId = produtoId,
                FornecedorId = 1,
                Quantidade = quantidade,
                QuantidadeDisponivel = quantidade,
                CustoUnitario = 0,
                DataEntrada = DateTime.UtcNow
            };

            context.ProdutoLotes.Add(lote);

            await auditLogService.LogAsync(
                "Estoque",
                "Movimentação de Entrada",
                $"Entrada de {quantidade} unidades do produto '{produto.Nome}' (ID: {produto.IdProduto}).",
                userId,
                userName
            );
        }

        await context.SaveChangesAsync();
    }


    //private async Task NotificarEstoqueBaixo(Produto produto, string? userId)
    //{
    //    var notificacao = new Notificacao
    //    {
    //        Mensagem =
    //            $"⚠️ Estoque do produto '{produto.Nome}' abaixo do mínimo! ({produto.QuantidadeEstoque} unidades restantes)",
    //        Data = LocalTime.Now(),
    //        IdUser = userId
    //    };

    //    context.Notificacoes.Add(notificacao);
    //    await context.SaveChangesAsync();
    //}
}