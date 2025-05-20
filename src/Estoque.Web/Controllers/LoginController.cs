using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
