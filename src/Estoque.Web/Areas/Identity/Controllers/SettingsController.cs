using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Web.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class SettingsController(UserManager<ApplicationUser> userManager, EstoqueDbContext context) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);

            var settings = await context.UserSettings
                .FirstOrDefaultAsync(s => s.UserId == userId)
                ?? new UserSettings { UserId = userId };

            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettings(UserSettings model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var settings = await context.UserSettings
                .FirstOrDefaultAsync(s => s.UserId == model.UserId);

            if (settings == null)
                context.UserSettings.Add(model);
            else
            {
                settings.DarkMode = model.DarkMode;
                settings.PreferredLanguage = model.PreferredLanguage;
                settings.EnableLanguageSwitch = model.EnableLanguageSwitch;
                settings.EnableDarkModeSwitch = model.EnableDarkModeSwitch;
            }

            await context.SaveChangesAsync();

            TempData["Success"] = "Configurações salvas com sucesso!";
            return RedirectToAction("Index");
        }
    }
}
