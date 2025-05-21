using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Controllers;

public class ProdutoController(ProdutoService produtoService) : Controller
{
    [Route("Produto")]
    public async Task<IActionResult> Produto()
    {
        var formViewProdutoAsync = await produtoService.GetFormViewProdutoAsync();
        
        var resultGridProdutoAsync = await formViewProdutoAsync.GetResultAsync();

        if (resultGridProdutoAsync is IActionResult actionResultGridProdutoAsync)
            return actionResultGridProdutoAsync;
        
        ViewBag.FormViewProduto = resultGridProdutoAsync.Content;
        return View();
    }
    
    [Route("Entrada")]
    public async Task<IActionResult> EntradaProduto()
    {
        var formViewEntradaAsync = await produtoService.GetFormViewEntradaAsync();

        var resultGridEntradaAsync = await formViewEntradaAsync.GetResultAsync();

        if (resultGridEntradaAsync is IActionResult actionResultGridEntradaAsync)
            return actionResultGridEntradaAsync;

        ViewBag.FormViewEntrada = resultGridEntradaAsync.Content;
        return View();
    }
}