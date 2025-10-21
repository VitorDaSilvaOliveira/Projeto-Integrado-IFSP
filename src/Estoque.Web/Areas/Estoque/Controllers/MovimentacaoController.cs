using Estoque.Infrastructure.Services;
using Estoque.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
[AuditLog("Estoque", "Menu", "Usuário acessou menu Movimentação")]
public class MovimentacaoController(MovimentacaoService movimentacaoService) : Controller
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