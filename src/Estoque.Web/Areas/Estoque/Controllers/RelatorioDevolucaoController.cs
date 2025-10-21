using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers
{
    [Area("Estoque")]
    public class RelatorioDevolucaoTrocaController : Controller
    {
        public IActionResult Index()
        {
            // Apenas para teste
            var testeDevolucoes = new List<string> { "Produto A", "Produto B", "Produto C" };
            return View(testeDevolucoes);
        }
    }
}