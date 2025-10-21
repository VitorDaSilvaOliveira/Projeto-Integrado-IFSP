using Estoque.Infrastructure.Services;
using Estoque.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Admin.Controllers;

[Area("Admin")]
[AuditLog("Estoque", "Menu", "Usuário acessou menu Auditoria")]
public class AuditLogController(AuditLogService auditLogService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var formViewLogAsync = await auditLogService.GetFormViewAuditLogAsync();
        var resultGridLogAsync = await formViewLogAsync.GetResultAsync();

        if (resultGridLogAsync is IActionResult actionResult)
            return actionResult;

        ViewBag.FormViewAuditLog = resultGridLogAsync.Content;

        return View();
    }
}