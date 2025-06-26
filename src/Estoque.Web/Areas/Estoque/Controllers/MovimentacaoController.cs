using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
public class MovimentacaoController(MovimentacaoService movimentacaoService, EstoqueDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var formViewMovimentacaoAsync = await movimentacaoService.GetFormViewMovimentacaoAsync();

        var resultGridMovimentacaoAsync = await formViewMovimentacaoAsync.GetResultAsync();

        if (resultGridMovimentacaoAsync is IActionResult actionResultGridMovimentacaoAsync)
            return actionResultGridMovimentacaoAsync;

        ViewBag.FormViewMovimentacao = resultGridMovimentacaoAsync.Content;
        return View();
    }
    
 


    
}
