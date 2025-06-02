using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
public class EntradaController(EntradaService entradaService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var formViewEntradaAsync = await entradaService.GetFormViewEntradaAsync();

        var resultGridEntradaAsync = await formViewEntradaAsync.GetResultAsync();

        if (resultGridEntradaAsync is IActionResult actionResultGridEntradaAsync)
            return actionResultGridEntradaAsync;

        ViewBag.FormViewEntrada = resultGridEntradaAsync.Content;
        return View();
    }
}
