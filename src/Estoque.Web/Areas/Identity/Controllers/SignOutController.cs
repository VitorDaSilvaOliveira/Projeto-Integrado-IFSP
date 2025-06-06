using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Estoque.Domain.Entities;

namespace Estoque.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class SignOutController(SignInManager<ApplicationUser> signInManager) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "SignIn");
    }
}