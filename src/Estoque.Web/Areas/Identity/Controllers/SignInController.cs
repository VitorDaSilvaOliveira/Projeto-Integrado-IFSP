using Estoque.Domain.Models;
using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class SignInController(AuthService authService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (success, errorMessage) = await authService.SignInAsync(model.Login, model.Senha, model.LembrarMe);

        if (success)
            return RedirectToAction("Index", "Home", new { area = "Estoque" });

        ModelState.AddModelError(string.Empty, errorMessage ?? "Login falhou.");
        return View(model);
    }
}