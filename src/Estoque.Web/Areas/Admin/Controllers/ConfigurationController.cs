using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using Estoque.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Web.Areas.Admin.Controllers;

[Area("Admin")]
[AuditLog("Estoque", "Menu", "Usuário acessou menu Painel de Administrador")]
public class ConfigurationController( UserService userService, EstoqueDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.ActiveUsersCount = await userService.GetActiveUsersCount();
        ViewBag.ItensBaixoEstoque = await context.Produtos.CountAsync(p => p.IdProduto == 1);
        ViewBag.PedidosPendentes = await context.Pedidos.CountAsync(p => p.Status == PedidoStatus.EmAndamento);

        return View();
    }
}