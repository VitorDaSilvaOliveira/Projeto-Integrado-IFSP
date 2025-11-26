using Estoque.Infrastructure.Services;
using Estoque.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
[AuditLog("Estoque", "Menu", "Usuário acessou menu Devoluções")]
public class DevolucaoController(DevolucaoService devolucaoService) : Controller
{
    [Route("Devolucao")]
    public async Task<IActionResult> Index()
    {
        var formViewDevolucaoAsync = await devolucaoService.GetFormViewDevolucaoAsync();

        var resultGridDevolucaoAsync = await formViewDevolucaoAsync.GetResultAsync();

        if (resultGridDevolucaoAsync is IActionResult actionResultGridDevolucaoAsync)
            return actionResultGridDevolucaoAsync;

        ViewBag.FormViewDevolucao = resultGridDevolucaoAsync.Content;
        return View();
    }

    public async Task<IActionResult> Report()
    {
        var formViewRelatorioDevolucaoAsync = await devolucaoService.GetFormViewReportDevolucaoAsync();

        var resultGridReportDevolucaoAsync = await formViewRelatorioDevolucaoAsync.GetResultAsync();

        if (resultGridReportDevolucaoAsync is IActionResult actionResultGridReportDevolucaoAsync)
            return actionResultGridReportDevolucaoAsync;

        ViewBag.FormViewRelatorioDevolucao = resultGridReportDevolucaoAsync.Content;
        return View();
    }
    public async Task<IActionResult> ViewReportDevolucaoPdf(int idDevolucao)
    {
        var pdf = await devolucaoService.ReportDevolucaoAsync(idDevolucao);

        if (pdf.Length == 0)
            return NotFound("PDF não pôde ser gerado.");

        return File(pdf, "application/pdf");
    }

    public async Task<IActionResult> DownloadReportDevolucaoPdf(int idDevolucao)
    {
        var pdf = await devolucaoService.ReportDevolucaoAsync(idDevolucao);

        if (pdf.Length == 0)
            return NotFound("PDF não pôde ser gerado.");

        return File(
            pdf,
            "application/pdf",
            $"pedido_{idDevolucao}.pdf"
        );
    }
}