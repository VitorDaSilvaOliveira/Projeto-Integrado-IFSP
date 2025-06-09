using JJMasterData.Core.UI.Components;

namespace Estoque.Infrastructure.Services;

public class LogService(IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewLogAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Log");
        formView.ShowTitle = true;
        return formView;
    }
}