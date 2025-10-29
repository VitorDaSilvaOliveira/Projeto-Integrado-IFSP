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
        string folder = Path.Combine(env.WebRootPath, "danfe");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, $"danfe-{pedidoId}.pdf");
        notaFiscalService.GenerateDanfe(pedidoId, path);

        var fileBytes = System.IO.File.ReadAllBytes(path);
        return File(fileBytes, "application/pdf");
    }

    public IActionResult Download(int pedidoId)
    {
        string folder = Path.Combine(env.WebRootPath, "danfe");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, $"danfe-{pedidoId}.pdf");
        notaFiscalService.GenerateDanfe(pedidoId, path);

        var fileBytes = System.IO.File.ReadAllBytes(path);
        return File(fileBytes, "application/pdf", $"danfe-{pedidoId}.pdf");
    }
}
