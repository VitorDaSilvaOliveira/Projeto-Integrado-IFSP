using Microsoft.AspNetCore.Mvc;
using Estoque.Infrastructure.Services;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
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
}