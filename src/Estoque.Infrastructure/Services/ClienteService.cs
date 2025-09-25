using JJMasterData.Core.UI.Components;

namespace Estoque.Infrastructure.Services;

public class ClienteService(IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewClienteAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Cliente");
        formView.ShowTitle = true;
        return formView;
    }

    public async Task<JJFormView> GetFormViewReportClienteAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("RelatorioCliente");
        formView.ShowTitle = true;
        return formView;
    }

}


