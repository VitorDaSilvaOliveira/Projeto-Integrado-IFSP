using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using JJMasterData.Core.Events.Args;
using JJMasterData.Core.UI.Components;

namespace Estoque.Infrastructure.Services;

public class PedidoService(IComponentFactory componentFactory, EstoqueDbContext context)
{
    public async Task<JJFormView> GetFormViewPedidoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Pedido");
        formView.ShowTitle = true;

        //formView.OnBeforeUpdateAsync += GetValuesPreUpdate;
        formView.OnAfterUpdateAsync += GetValuesPostUpdate;

        return formView;
    }

    public async Task<JJFormView> GetFormViewReportPedidoAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("RelatorioPedido");
        formView.ShowTitle = true;
        return formView;
    }


    public async ValueTask GetValuesPreUpdate(object sender, FormAfterActionEventArgs e)
    {
        var values = e.Values;

        var produtoId = Convert.ToInt32(values["Id_Produto"]);
        var quantidade = Convert.ToInt32(values["Quantidade"]);
        var tipoMovimentacao = (TipoMovimentacao)Convert.ToInt32(values["TipoMovimentacao"]);
        var userId = values["Id_User"]?.ToString();
    }

    public async ValueTask GetValuesPostUpdate(object sender, FormAfterActionEventArgs e)
    {
        var values = e.Values;

        var pedidoId = Convert.ToInt32(values["Id"]);
        var userId = values["Id_User"]?.ToString();
        var statusPedido = Convert.ToInt32(values["Status"]);

        List<PedidoItem> pedidosItem = context.PedidosItens
            .Where(pi => pi.Id == pedidoId)
            .ToList();

    }


}