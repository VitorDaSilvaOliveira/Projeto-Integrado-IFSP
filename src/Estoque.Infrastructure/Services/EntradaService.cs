using JJMasterData.Core.UI.Components;

namespace Estoque.Infrastructure.Services;

public class EntradaService(IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewEntradaAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("entradas");
        formView.ShowTitle = true;
        return formView;
    }
}
