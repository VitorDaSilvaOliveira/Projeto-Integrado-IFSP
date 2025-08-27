using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ConfigurationController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
