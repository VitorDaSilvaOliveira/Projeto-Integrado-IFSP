using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using JJMasterData.Core.Events.Abstractions;
using JJMasterData.Core.Events.Args;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Estoque.Infrastructure.Services
{
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

            formView.OnAfterUpdateAsync += OnAfterUpdateAsync;

            return formView;
        }

        public async ValueTask OnAfterUpdateAsync(object sender, FormAfterActionEventArgs e)
        {
            var values = e.Values;

            Devolucao devolucao = await context.Devolucoes.FindAsync(Convert.ToInt32(values["idDevolucao"]));

            var itensDevolucao = context.DevolucoesItens
                .Where(di => di.IdDevolucao == devolucao.IdDevolucao)
                .ToList();

            foreach (var item in itensDevolucao)
            {
                var produto = await context.Produtos.FindAsync(item.IdProduto);
                if (produto != null)
                {

                    if(item.Devolvido == 1) // Verifica se já foi devolvido
                        continue;

                    var movimentacao = new Movimentacao
                    {
                        IdProduto = item.IdProduto,
                        Quantidade = item.QuantidadeDevolvida,
                        TipoMovimentacao = TipoMovimentacao.Entrada,
                        DataMovimentacao = DateTime.Now.Date, // armazena somente a data
                        Observacao = item.Motivo,
                        IdUser = devolucao.IdUser
                    };
                    context.Movimentacoes.Add(movimentacao);
                    await movimentacaoService.RegistrarMovimentacaoAsync(produto.IdProduto, item.QuantidadeDevolvida, TipoMovimentacao.Entrada, devolucao.IdUser);

                    item.Devolvido = 1;

                    await context.SaveChangesAsync();
                }
            }
        }

    }
}
