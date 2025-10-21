using Estoque.Infrastructure.Services;
using Estoque.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
[AuditLog("Estoque", "Menu", "Usuário acessou menu Pedidos")]
public class PedidoController(PedidoService pedidoService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var formViewPedidoAsync = await pedidoService.GetFormViewPedidoAsync();

        var resultGridPedidoAsync = await formViewPedidoAsync.GetResultAsync();

        if (resultGridPedidoAsync is IActionResult actionResultGridPedidoAsync)
            return actionResultGridPedidoAsync;

        ViewBag.FormViewPedido = resultGridPedidoAsync.Content;
        return View();
    }

    public async Task<IActionResult> Report()
    {
        var formViewRelatorioPedidoAsync = await pedidoService.GetFormViewReportPedidoAsync();

        var resultGridReportPedidoAsync = await formViewRelatorioPedidoAsync.GetResultAsync();

        if (resultGridReportPedidoAsync is IActionResult actionResultGridReportPedidoAsync)
            return actionResultGridReportPedidoAsync;

        ViewBag.FormViewRelatorioPedido = resultGridReportPedidoAsync.Content;
        return View();
    }

    public async Task<IActionResult> SendOrder(int idPedido, string userId)
    {
        await pedidoService.ProcessOrder(idPedido, userId);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Dashboard() => View();

    [HttpGet]
    public async Task<IActionResult> GetPedidosPorOperacao()
    {
        var dados = await pedidoService.ObterPedidosPorOperacaoAsync();
        return Json(dados);
    }
}