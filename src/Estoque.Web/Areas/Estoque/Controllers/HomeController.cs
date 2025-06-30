using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
public class HomeController(HomeService homeService) : Controller
{
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetProdutosPorCategoria()
    {
        var data = await homeService.GetProdutosPorCategoriaAsync();
        return Json(new { produtosPorCategoria = data });
    }

    [HttpGet]
    public async Task<IActionResult> GetMovimentacoesUltimosMeses()
    {
        var (labels, entradas, saidas) = await homeService.GetMovimentacoesUltimosMesesAsync();
        return Json(new { labels, entradas, saidas });
    }

    [HttpGet]
    public async Task<IActionResult> GetEstoqueMensal()
    {
        var data = await homeService.GetEstoqueMensalAsync();
        return Json(data);
    }
}