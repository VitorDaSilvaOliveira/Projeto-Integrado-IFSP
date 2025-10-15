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
    public string ElementName => "Movimentacao";

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

        if (values.ContainsKey("Quantidade"))
        {
            quantidade = Convert.ToInt32(values["Quantidade"]);
        }

        await RegistrarMovimentacaoAsync(produtoId, quantidade, tipoMovimentacao, userId, "Movimentação via formulário");
    }

    // ✅ MÉTODO 1: RegistrarSaidaAsync - CORRIGIDO
    public async Task<bool> RegistrarSaidaAsync(int produtoId, int quantidade, string observacao, int? pedidoItemId = null)
    {
        // ✅ CORREÇÃO: Proteger acesso ao Context
        string userId = null;
        string userName = "Sistema";

        try
        {
            var user = signInManager.Context?.User;
            if (user != null)
            {
                userId = signInManager.UserManager.GetUserId(user);
                userName = user?.Identity?.Name ?? "Sistema";
            }
        }
        catch (Exception)
        {
            // Em ambiente de teste, Context pode ser null
            userName = "Sistema/Teste";
        }

        try
        {
            await RegistrarMovimentacaoAsync(produtoId, quantidade, TipoMovimentacao.Saida, userId, observacao, pedidoItemId);
            return true;
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Erro ao registrar saída para o produto {ProdutoId}: {Mensagem}", produtoId, ex.Message);
            return false;
        }
    }

    // ✅ MÉTODO 2: RegistrarMovimentacaoAsync - CORRIGIDO
    public async Task RegistrarMovimentacaoAsync(
        int produtoId,
        int quantidade,
        TipoMovimentacao tipoMovimentacao,
        string? userId,
        string observacao,
        int? pedidoItemId = null)
    {
        var produto = await context.Produtos
            .Include(p => p.ProdutoLotes)
            .FirstOrDefaultAsync(p => p.IdProduto == produtoId);

        if (produto == null)
        {
            logger.LogError("Produto não encontrado.");
            throw new InvalidOperationException("Produto não encontrado.");
        }

        // ✅ CORREÇÃO: Proteger acesso ao Context
        string userName = "Sistema";
        try
        {
            var user = signInManager.Context?.User;
            userName = user?.Identity?.Name ?? "Anônimo";
        }
        catch (Exception)
        {
            // Em ambiente de teste, Context pode ser null
            userName = "Sistema/Teste";
        }

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

            // ✅ ADICIONAR: Criar registro de movimentação
            var movimentacao = new Movimentacao
            {
                IdProduto = produtoId,
                Quantidade = quantidade,
                TipoMovimentacao = TipoMovimentacao.Saida,
                DataMovimentacao = DateTime.UtcNow,
                Observacao = observacao,
                IdUser = userId
            };
            context.Movimentacoes.Add(movimentacao);

            await auditLogService.LogAsync(
                "Estoque",
                "Movimentação de Saída",
                $"Saída de {quantidade} unidades do produto '{produto.Nome}' (ID: {produto.IdProduto}). {observacao}",
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

            // ✅ ADICIONAR: Criar registro de movimentação
            var movimentacao = new Movimentacao
            {
                IdProduto = produtoId,
                Quantidade = quantidade,
                TipoMovimentacao = TipoMovimentacao.Entrada,
                DataMovimentacao = DateTime.UtcNow,
                Observacao = observacao,
                IdUser = userId
            };
            context.Movimentacoes.Add(movimentacao);

            await auditLogService.LogAsync(
                "Estoque",
                "Movimentação de Entrada",
                $"Entrada de {quantidade} unidades do produto '{produto.Nome}' (ID: {produto.IdProduto}). {observacao}",
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