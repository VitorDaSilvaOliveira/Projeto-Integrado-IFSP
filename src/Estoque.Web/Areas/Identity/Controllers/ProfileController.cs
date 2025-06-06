using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class ProfileController : Controller
{
    public IActionResult Index()
    {
        var claims = User.Claims.ToList();

        return View();
    }
}