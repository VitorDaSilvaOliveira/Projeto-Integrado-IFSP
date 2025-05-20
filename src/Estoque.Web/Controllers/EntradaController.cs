using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Controllers;

public class EntradaController(EntradaService entradaService) : Controller
{
    [Route("Entrada")]
    public async Task<IActionResult> Entrada()
    {
        var formViewEntradaAsync = await entradaService.GetFormViewEntradaAsync();

        var resultGridEntradaAsync = await formViewEntradaAsync.GetResultAsync();

        if (resultGridEntradaAsync is IActionResult actionResultGridEntradaAsync)
            return actionResultGridEntradaAsync;

        ViewBag.FormViewEntrada = resultGridEntradaAsync.Content;
        return View();
    }
}