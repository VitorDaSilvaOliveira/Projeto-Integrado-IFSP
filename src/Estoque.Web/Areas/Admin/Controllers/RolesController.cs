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
public class RolesController(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    RoleService roleService)
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
        var vm = await roleService.BuildViewModelAsync(roleId);
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

            switch (userVm.IsSelected)
            {
                case true when !await userManager.IsInRoleAsync(user, role.Name):
                    await userManager.AddToRoleAsync(user, role.Name);
                    break;
                case false when await userManager.IsInRoleAsync(user, role.Name):
                    await userManager.RemoveFromRoleAsync(user, role.Name);
                    break;
            }
        }

        await roleService.UpdateRoleMenusAsync(model.RoleId, model.Groups);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> LoadMore(int page, string? searchText)
    {
        var vm = await GetRolesViewModel();
        return PartialView("_Rows", vm);
    }
}