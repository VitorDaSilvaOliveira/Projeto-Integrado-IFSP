using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
public class CategoriaController(CategoriaService categoriaService) : Controller
{
    [Route("Categoria")]
    public async Task<IActionResult> Index()
    {
        var formViewCategoriaAsync = await categoriaService.GetFormViewCategoriaAsync();
        var resultGridCategoriaAsync = await formViewCategoriaAsync.GetResultAsync();

        if (resultGridCategoriaAsync is IActionResult actionResult)
            return actionResult;

        ViewBag.FormViewCategoria = resultGridCategoriaAsync.Content;
        return View();

    }
}
