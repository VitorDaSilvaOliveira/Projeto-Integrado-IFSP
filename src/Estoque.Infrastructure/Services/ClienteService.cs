using Estoque.Infrastructure.Repository;
using JJMasterData.Core.UI.Components;

namespace Estoque.Infrastructure.Services;

public class ClienteService(IComponentFactory componentFactory, ClienteRepository clienteRepository)
{
    public async Task<JJFormView> GetFormViewClienteAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Cliente");
        formView.ShowTitle = true;
        return formView;
    }
    public async Task<object> DashboardClienteAsync()
    {
        return await clienteRepository.DashboardClienteAsync();
    }
}


