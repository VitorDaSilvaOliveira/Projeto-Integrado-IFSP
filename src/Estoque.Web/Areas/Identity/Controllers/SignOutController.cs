using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Estoque.Domain.Entities;
using Estoque.Infrastructure.Services;

namespace Estoque.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class SignOutController(
    SignInManager<ApplicationUser> signInManager,
    AuditLogService auditLogService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var userName = User.Identity?.Name ?? "Anônimo";
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await auditLogService.LogAsync(
            area: "Identity",
            action: "Logout",
            details: "Usuário se deslogou",
            userId: userId,
            userName: userName
        );

        await signInManager.SignOutAsync();

        return RedirectToAction("Index", "SignIn");
    }
}