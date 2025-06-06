using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Estoque.Domain.Entities;

namespace Estoque.Web.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class SignOutController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SignOutController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index()
        {
            await _signInManager.SignOutAsync(); 
            return RedirectToAction("Index", "SignIn"); 
        }
    }
}
