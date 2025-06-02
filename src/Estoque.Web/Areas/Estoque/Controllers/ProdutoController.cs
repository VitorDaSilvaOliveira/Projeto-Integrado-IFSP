using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
public class ProdutoController(ProdutoService produtoService) : Controller
{
    [Route("Produto")]
    public async Task<IActionResult> Index()
    {
        var formViewProdutoAsync = await produtoService.GetFormViewProdutoAsync();
        
        var resultGridProdutoAsync = await formViewProdutoAsync.GetResultAsync();

        if (resultGridProdutoAsync is IActionResult actionResultGridProdutoAsync)
            return actionResultGridProdutoAsync;
        
        ViewBag.FormViewProduto = resultGridProdutoAsync.Content;
        return View();
    }
}