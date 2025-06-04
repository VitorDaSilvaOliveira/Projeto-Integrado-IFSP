using JJMasterData.Core.UI.Components;

namespace Estoque.Infrastructure.Services;

public class FornecedorService(IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewFornecedorAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Fornecedor");
        formView.ShowTitle = true;
        return formView;
    }
}