using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Controllers;

public class HomeController : Controller
{
    [Route("Home")]
    public IActionResult Home()
    {
        return View();
    }

}