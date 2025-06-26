using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
public class NotificacaoController(EstoqueDbContext context, UserManager<ApplicationUser> userManager)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetNotificacoes()
    {
        var userId = userManager.GetUserId(User);

        var notificacoes = await context.Notificacoes
            .Where(n => n.IdUser == userId)
            .OrderByDescending(n => n.Data)
            .Take(5)
            .ToListAsync();

        return PartialView("_NotificacoesDropdown", notificacoes);
    }
}