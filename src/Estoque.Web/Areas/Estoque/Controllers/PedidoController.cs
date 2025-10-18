using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
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
        try
        {
            await pedidoService.ProcessOrder(idPedido, userId);
            TempData["Success"] = "Pedido confirmado com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Dashboard() => View();

    [HttpGet]
    public async Task<IActionResult> GetPedidosPorOperacao()
    {
        var (labels, vendas, trocas) = await pedidoService.GetPedidosPorOperacaoAsync();
        return Json(new { labels, vendas, trocas });
    }
}