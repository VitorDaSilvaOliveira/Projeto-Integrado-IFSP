using JJMasterData.Core.UI.Components;

namespace Estoque.Infrastructure.Services;

public class MovimentacaoService(IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewMovimentacaoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Movimentacao");
        formView.ShowTitle = true;
        return formView;
    }
}
