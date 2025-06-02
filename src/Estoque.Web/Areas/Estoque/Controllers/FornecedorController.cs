using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
public class FornecedorController(FornecedorService fornecedorService)  : Controller
{
    [Route("Fornecedor")]
    public async Task<IActionResult> Index()
    {
        var formViewFornecedorAsync = await fornecedorService.GetFormViewFornecedorAsync();
        
        var resultGridFornecedorAsync = await formViewFornecedorAsync.GetResultAsync();

        if (resultGridFornecedorAsync is IActionResult actionResultGridFornecedorAsync)
            return actionResultGridFornecedorAsync;
        
        ViewBag.FormViewFornecedor = resultGridFornecedorAsync.Content;
        return View();
    }
}