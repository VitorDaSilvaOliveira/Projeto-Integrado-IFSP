using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Utils;
using JJMasterData.Core.Events.Abstractions;
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
    public string ElementName => "Movimentacao";

    // Adicionado 'virtual' para permitir Mock nos testes
    public virtual async Task<JJFormView> GetFormViewMovimentacaoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Movimentacao");
        formView.ShowTitle = true;

        return formView;
    }

    // AQUI ESTAVA O ERRO: Adicionado 'virtual'
    public virtual async Task RegistrarMovimentacaoAsync(
       int produtoId,
       int quantidade,
       TipoMovimentacao tipoMovimentacao,
       string? userId,
       string obs)
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
                DataMovimentacao = LocalTime.Now(),
                IdUser = userId,
                Observacao = obs
            };

            context.Movimentacoes.Add(entradaResgistrada);

            await auditLogService.LogAsync(
                "Estoque",
                "Movimentação de Saída",
                $"Saída de {quantidade} unidades do produto '{produto.Nome}' (Cód: {produto.Codigo}).",
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
                DataEntrada = LocalTime.Now()
            };

            context.ProdutoLotes.Add(lote);

            await auditLogService.LogAsync(
                "Estoque",
                "Movimentação de Entrada",
                $"Entrada de {quantidade} unidades do produto '{produto.Nome}' (Cód: {produto.Codigo}).",
                userId,
                userName
            );
        }
        else if (tipoMovimentacao == TipoMovimentacao.Devolucao)
        {
            var devolucao = new Movimentacao
            {
                IdProduto = produtoId,
                Quantidade = quantidade,
                TipoMovimentacao = TipoMovimentacao.Devolucao,
                DataMovimentacao = LocalTime.Now(),
                IdUser = userId,
                Observacao = obs
            };

            context.Movimentacoes.Add(devolucao);

            await auditLogService.LogAsync(
                "Estoque",
                "Movimentação de Devolução",
                $"Devolução de {quantidade} unidades do produto '{produto.Nome}' (Cód:  {produto.Codigo} ).",
                userId,
                userName
            );
        }

        await context.SaveChangesAsync();
    }
}