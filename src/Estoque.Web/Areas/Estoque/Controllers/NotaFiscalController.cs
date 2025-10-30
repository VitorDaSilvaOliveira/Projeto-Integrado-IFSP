using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
public class NotaFiscalController(NotaFiscalService notaFiscalService, IWebHostEnvironment env) : Controller
{

    public async Task<IActionResult> Index()
    {
        var formViewNotaFiscalAsync = await notaFiscalService.GetFormViewNotaFiscalAsync();

        var resultGridNotaFiscalAsync = await formViewNotaFiscalAsync.GetResultAsync();

        if (resultGridNotaFiscalAsync is IActionResult actionResultGridnotaFiscalAsync)
            return actionResultGridnotaFiscalAsync;

        ViewBag.FormViewNotaFiscal = resultGridNotaFiscalAsync.Content;
        return View();
    }

    public IActionResult ViewDocument(int pedidoId)
    {
        using var ms = new MemoryStream();
        notaFiscalService.GenerateDanfe(pedidoId, ms);
        ms.Position = 0;

        Response.Headers.Append("Content-Disposition", $"inline; filename=danfe-{pedidoId}.pdf");
        return File(ms.ToArray(), "application/pdf");
    }

    public IActionResult Download(int pedidoId)
    {
        using var ms = new MemoryStream();
        notaFiscalService.GenerateDanfe(pedidoId, ms); 
        ms.Position = 0;

        return File(ms.ToArray(), "application/pdf", $"danfe-{pedidoId}.pdf");
    }
}
