using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using JJMasterData.Core.Events.Abstractions;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
namespace Estoque.Infrastructure.Services
{
    public class DevolucaoItemService(
    IComponentFactory componentFactory,
    EstoqueDbContext context,
    ILogger<MovimentacaoService> logger,
    AuditLogService auditLogService,
    SignInManager<ApplicationUser> signInManager) : IFormEventHandler
    {
        public async Task<JJFormView> GetFormViewDevolucaoItemAsync()
        {
            var formView = await componentFactory.FormView.CreateAsync("Devolucao-Item");
            formView.ShowTitle = true;

            //formView.OnAfterInsertAsync += OnAfterInsertAsync;
            //formView.OnAfterUpdateAsync += OnAfterUpdateAsync;

            return formView;
        }

    }
}
