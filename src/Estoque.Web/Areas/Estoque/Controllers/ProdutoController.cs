using Estoque.Infrastructure.Services;
using Estoque.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
[AuditLog("Estoque", "Menu", "Usuário acessou menu Produtos")]
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