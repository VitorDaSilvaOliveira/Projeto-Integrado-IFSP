using Estoque.Domain.Entities;
using Estoque.Domain.Models;
using Estoque.Infrastructure.Services;
using Estoque.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class RolesController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, RoleService roleService)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        var vm = await GetRolesViewModel();
        return View(vm);
    }

    private async Task<RolesIndexViewModel> GetRolesViewModel()
    {
        var roles = await roleManager.Roles
                                     .OrderBy(r => r.Name)
                                     .ToListAsync();

        var rolesWithUsers = new List<RoleWithUsersViewModel>();

        foreach (var role in roles)
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
            rolesWithUsers.Add(new RoleWithUsersViewModel
            {
                Role = role,
                Users = usersInRole.ToList()
            });
        }

        return new RolesIndexViewModel
        {
            RolesWithUsers = rolesWithUsers
        };
    }

    [HttpPost]
    public async Task<IActionResult> Create(string newRoleName)
    {
        if (string.IsNullOrWhiteSpace(newRoleName))
            return BadRequest(new { errors = new[] { "O nome do perfil é obrigatório." } });

        if (await roleManager.RoleExistsAsync(newRoleName))
            return BadRequest(new { errors = new[] { "Esse perfil já existe." } });

        var result = await roleManager.CreateAsync(new ApplicationRole(newRoleName));

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        if (string.IsNullOrWhiteSpace(roleId))
            return BadRequest("ID inválido.");

        var role = await roleManager.FindByIdAsync(roleId);
        if (role == null)
            return NotFound("Perfil não encontrado.");

        if (role.Name is "Admin" or "Guest")
            return BadRequest("Esse perfil não pode ser excluído.");

        var result = await roleManager.DeleteAsync(role);
        if (!result.Succeeded)
            return BadRequest("Erro ao deletar o perfil.");

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> RoleDetails(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role == null) return NotFound();

        var allUsers = userManager.Users.ToList();
        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);

        var allMenus = new List<EditMenuViewModel>
        {
            new() { MenuId = "Home",       MenuName = "Dashboards",   IsSelected = await roleService.HasAccessAsync(role.Id, "Home") },
            new() { MenuId = "Produto",    MenuName = "Produtos",     IsSelected = await roleService.HasAccessAsync(role.Id, "Produto") },
            new() { MenuId = "Fornecedor", MenuName = "Fornecedores", IsSelected = await roleService.HasAccessAsync(role.Id, "Fornecedor") },
            new() { MenuId = "Categoria",  MenuName = "Categorias",   IsSelected = await roleService.HasAccessAsync(role.Id, "Categoria") },
            new() { MenuId = "Cliente",    MenuName = "Clientes",     IsSelected = await roleService.HasAccessAsync(role.Id, "Cliente") },
            new() { MenuId = "Pedido",     MenuName = "Pedidos",      IsSelected = await roleService.HasAccessAsync(role.Id, "Pedido") },
            new() { MenuId = "EntradaSaida", MenuName = "Entrada e Saídas", IsSelected = await roleService.HasAccessAsync(role.Id, "EntradaSaida") },
            new() { MenuId = "RelatorioPedido", MenuName = "Relatório de Pedidos", IsSelected = await roleService.HasAccessAsync(role.Id, "RelatorioPedido") }
        };

        var vm = new EditRoleViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name!,
            Users = allUsers.Select(u => new UserRoleViewModel
            {
                UserId = u.Id,
                UserName = u.UserName!,
                IsSelected = usersInRole.Any(x => x.Id == u.Id)
            }).ToList(),
            Menus = allMenus
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RoleDetails(EditRoleViewModel model)
    {
        var role = await roleManager.FindByIdAsync(model.RoleId);
        if (role == null) return NotFound();

        role.Name = model.RoleName;
        role.LastModifiedDate = LocalTime.Now();

        var updateResult = await roleManager.UpdateAsync(role);
        if (!updateResult.Succeeded)
        {
            ModelState.AddModelError("", "Erro ao atualizar o perfil.");
            return View(model);
        }

        foreach (var userVm in model.Users)
        {
            var user = await userManager.FindByIdAsync(userVm.UserId);
            if (user == null) continue;

            if (userVm.IsSelected && !await userManager.IsInRoleAsync(user, role.Name))
                await userManager.AddToRoleAsync(user, role.Name);

            if (!userVm.IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                await userManager.RemoveFromRoleAsync(user, role.Name);
        }

        await roleService.UpdateRoleMenusAsync(model.RoleId, model.Menus);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> LoadMore(int page, string? searchText)
    {
        var vm = await GetRolesViewModel();
        return PartialView("_Rows", vm);
    }
}