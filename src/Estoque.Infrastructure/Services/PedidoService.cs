using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using JJMasterData.Core.Events.Args;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Estoque.Infrastructure.Services;

public class PedidoService(IComponentFactory componentFactory,
    EstoqueDbContext context,
    ILogger<MovimentacaoService> logger,
    AuditLogService auditLogService,
    SignInManager<ApplicationUser> signInManager)
{
    public int? statusPreUpdate = null;
    public int? statusPostUpdate = null;

    public async Task<JJFormView> GetFormViewPedidoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Pedido");
        formView.ShowTitle = true;

        formView.OnBeforeUpdateAsync += GetStatusBeforeUpdate;
        formView.OnAfterUpdateAsync += GetStatusAfterUpdate;

        return formView;
    }

    public async Task<JJFormView> GetFormViewReportPedidoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("RelatorioPedido");
        formView.ShowTitle = true;
        return formView;
    }


    // MÉTODOS DE UPDATE

    public async ValueTask GetStatusBeforeUpdate(object sender, FormBeforeActionEventArgs e)
    {
        var values = e.Values;

        statusPreUpdate = Convert.ToInt32(values["Status"]);

        //await RegistrarMovimentacaoAsync(produtoId, quantidade, tipoMovimentacao, userId);
    }

    public async ValueTask GetStatusAfterUpdate(object sender, FormAfterActionEventArgs e)
    {
        var values = e.Values;

        statusPostUpdate = Convert.ToInt32(values["Status"]);

        //await RegistrarMovimentacaoAsync(produtoId, quantidade, tipoMovimentacao, userId);

        if (statusPreUpdate == 0 && statusPostUpdate == 1)
        {
            int a = 1;
        }

    }

}