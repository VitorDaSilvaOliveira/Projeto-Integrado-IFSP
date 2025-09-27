using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
public class ClienteController(ClienteService clienteService) : Controller
{
    [Route("Cliente")]
    public async Task<IActionResult> Index()
    {
        var formViewClienteAsync = await clienteService.GetFormViewClienteAsync();
        var resultGridClienteAsync = await formViewClienteAsync.GetResultAsync();

        if (resultGridClienteAsync is IActionResult actionResult)
            return actionResult;

        ViewBag.FormViewCliente = resultGridClienteAsync.Content;
        return View();

    }


    public async Task<IActionResult> Report()
    {
        var formViewRelatorioClienteAsync = await clienteService.GetFormViewReportClienteAsync();

        var resultGridReportClienteAsync = await formViewRelatorioClienteAsync.GetResultAsync();
        if (resultGridReportClienteAsync is IActionResult actionResultGridReportClienteAsync)
            return actionResultGridReportClienteAsync;

        ViewBag.FormViewRelatorioCliente = resultGridReportClienteAsync.Content;
        return View();
    }

}


