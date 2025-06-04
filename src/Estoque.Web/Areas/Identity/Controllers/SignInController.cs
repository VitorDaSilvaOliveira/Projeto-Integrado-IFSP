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

    [AllowAnonymous]
    [HttpGet]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleResponse), "SignIn", new { area = "Identity" }, protocol: Request.Scheme, host: Request.Host.Value);

        var properties = authService.ConfigureExternalAuthenticationProperties("Google", redirectUrl!);

        return new ChallengeResult("Google", properties);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await authService.ExternalLoginSignInAsync();

        if (result.Succeeded)
            return RedirectToAction("Index", "Home", new { area = "Estoque" });

        ModelState.AddModelError(string.Empty, "Falha ao autenticar com Google.");
        return View("Index");
    }

}