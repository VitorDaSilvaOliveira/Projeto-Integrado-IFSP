using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Entrar(string usuario, string senha)
        {
            return Redirect("/Produto");
        }
    }
}
