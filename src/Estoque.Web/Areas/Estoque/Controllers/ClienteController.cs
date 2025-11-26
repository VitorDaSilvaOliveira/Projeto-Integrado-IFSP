using Estoque.Infrastructure.Services;
using Estoque.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
[AuditLog("Estoque", "Menu", "Usuário acessou menu Clientes")]
public class ClienteController(ClienteService clienteService) : Controller
{
    [Route("Cliente")]
    public async Task<IActionResult> Index()
    {
        var formViewClienteAsync = await clienteService.GetFormViewClienteAsync();
        var resultGridClienteAsync = await formViewClienteAsync.GetResultAsync();

        if (resultGridClienteAsync is IActionResult actionResult)
            return actionResult;

        ViewBag.FormViewCliente = resultGridClienteAsync.Content;
        return View();
    }

    public IActionResult Dashboard() => View();
    
    [HttpGet]
    public async Task<IActionResult> GetDashboardData()
    {
        var dados = await clienteService.DashboardClienteAsync();
        return Json(dados);
    }
}