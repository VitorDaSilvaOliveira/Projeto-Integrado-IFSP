using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ConfigurationController(UserService userService) : Controller
{
    public async Task<IActionResult> IndexAsync()
    {
        ViewBag.ActiveUsersCount = await userService.GetActiveUsersCount();

        return View();
    }
}
