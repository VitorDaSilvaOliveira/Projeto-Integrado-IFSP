using Estoque.Infrastructure.Services;
using Estoque.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
[AuditLog("Estoque", "Menu", "Usuário acessou menu Pedidos")]
public class PedidoController(PedidoService pedidoService, RelatorioService relatorioService) : Controller
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

    [HttpGet]
    public IActionResult GeraPDFSLAConsumo(string idPedido)
    {
        var pedidosItens = new List<PedidoItemPDF>
            {
                new PedidoItemPDF
                {
                    nomeCodigoProduto = "IPHONE13-BLK",
                    descricaoProduto = "iPhone 13 Preto",
                    precoVendaPedidoItem = 3600,
                    categoriaProduto = "Celular",
                    fimGarantiaProdutoItem = "26/09/2026",
                    quantidadeProdutoItem = 1
                }
            };

        var pdf = relatorioService.GeraPDFSLAConsumo(pedidosItens);

        using var stream = new MemoryStream();
        pdf.Save(stream, false);
        stream.Position = 0;

        return File(stream.ToArray(), "application/pdf");
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