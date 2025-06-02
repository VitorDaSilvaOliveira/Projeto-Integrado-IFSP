using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

}