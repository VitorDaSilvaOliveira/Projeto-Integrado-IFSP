using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using JJMasterData.Core.Events.Abstractions;
using JJMasterData.Core.Events.Args;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Infrastructure.Services
{
    internal class SolicitacaoDevolucaoService(
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
            formView.OnAfterUpdateAsync += OnAfterUpdateAsync;

            return formView;
        }

        public async ValueTask OnAfterInsertAsync(object sender, FormAfterActionEventArgs e)
        {
        }

        public async ValueTask OnAfterUpdateAsync(object sender, FormAfterActionEventArgs e)
        {

            var values = e.Values;

            var produtoId = Convert.ToInt32(values["IdProduto"]);
            var status = Convert.ToInt32(values["IdStatus"]);
            bool devolvido = Convert.ToBoolean(values["Devolvido"]);
            var quantidade = Convert.ToInt32(values["Quantidade"]);
            var userId = values["Id_User"]?.ToString();

            await Devolucao(produtoId, status, devolvido, quantidade);
        }

        public async Task Devolucao(int produtoId, int status, bool devolvido, int quantidade)
        {
            if(devolvido)
            {
                return;
            }

            if(status == 3)
            {

                var produto = await context.Produtos.FindAsync(produtoId);

                if (produto == null)
                {
                    logger.LogError("Produto não encontrado.");
                    return;
                }
                produto.QuantidadeEstoque ??= 0;
                produto.QuantidadeEstoque += quantidade;
                await context.SaveChangesAsync();
            }
        }
    }
}
