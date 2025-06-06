using JJMasterData.Core.UI.Components;

namespace Estoque.Infrastructure.Services;

public class CategoriaService(IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewCategoriaAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Categoria");
        formView.ShowTitle = true;
        return formView;
    }
}
