using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Admin.Controllers;

[Area("Admin")]
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