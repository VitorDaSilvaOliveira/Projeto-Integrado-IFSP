using Estoque.Infrastructure.Services;
using Estoque.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Admin.Controllers;

[Area("Admin")]
[AuditLog("Estoque", "Menu", "Usuário acessou menu Log")]
public class LogController(LogService logService) : Controller
{
    [Route("Log")]
    public async Task<IActionResult> Index()
    {
        var formViewLogAsync = await logService.GetFormViewLogAsync();
        var resultGridLogAsync = await formViewLogAsync.GetResultAsync();

        if (resultGridLogAsync is IActionResult actionResult)
            return actionResult;

        ViewBag.FormViewLog = resultGridLogAsync.Content;
        return View();
    }
}