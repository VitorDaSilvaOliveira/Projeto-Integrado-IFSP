using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Estoque.Domain.Models;

namespace Estoque.Web.Controllers;

public class LoginController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
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
            {
                return RedirectToAction("Index", "Home");
            }

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

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Login", "Login");
    }
}