
using JJMasterData.Core.UI.Components;

namespace Estoque.Infrastructure.Services;

public class PedidoService(IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewPedidoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Pedido");
        formView.ShowTitle = true;
        return formView;
    }
}
