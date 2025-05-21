using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Controllers;

public class FornecedorController(FornecedorService fornecedorService)  : Controller
{
    [Route("Fornecedor")]
    public async Task<IActionResult> Fornecedor()
    {
        var formViewFornecedorAsync = await fornecedorService.GetFormViewFornecedorAsync();
        
        var resultGridFornecedorAsync = await formViewFornecedorAsync.GetResultAsync();

        if (resultGridFornecedorAsync is IActionResult actionResultGridFornecedorAsync)
            return actionResultGridFornecedorAsync;
        
        ViewBag.FormViewFornecedor = resultGridFornecedorAsync.Content;
        return View();
    }
}