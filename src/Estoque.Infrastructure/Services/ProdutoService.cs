using JJMasterData.Core.UI.Components;

namespace Estoque.Infrastructure.Services;

public class ProdutoService(IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewProdutoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Produto");
        formView.ShowTitle = true;
        return formView;
    }
}