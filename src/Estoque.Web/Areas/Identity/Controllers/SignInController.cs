using Estoque.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class SignInController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    : Controller
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

        var user = await userManager.FindByNameAsync(model.Usuario)
                   ?? await userManager.FindByEmailAsync(model.Usuario);

        if (user != null)
        {
            var result = await signInManager.PasswordSignInAsync(
                user,
                model.Senha,
                model.LembrarMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
                return RedirectToAction("Index", "Home", new { area = "Estoque" });

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Usuário bloqueado.");
                return View(model);
            }
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Usuário não autorizado a fazer login.");
                return View(model);
            }
        }

        ModelState.AddModelError("", "Tentativa de login inválida.");
        return View(model);
    }
}
