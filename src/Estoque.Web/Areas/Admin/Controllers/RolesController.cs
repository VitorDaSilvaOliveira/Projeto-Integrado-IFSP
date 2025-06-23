using Estoque.Domain.Entities;
using Estoque.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class RolesController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        var vm = await GetRolesViewModel();
        return View(vm);
    }

    private async Task<RolesIndexViewModel> GetRolesViewModel()
    {
        var roles = roleManager.Roles.ToList();

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
    [Authorize(Roles = "Admin")]
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

    public async Task<IActionResult> RoleDetails(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role == null)
            return NotFound();

        var users = await userManager.GetUsersInRoleAsync(role.Name);

        var vm = new RoleWithUsersViewModel
        {
            Role = role,
            Users = users.ToList()
        };

        return View(vm);
    }

    public async Task<IActionResult> LoadMore(int page, string? searchText)
    {
        var vm = await GetRolesViewModel();
        return PartialView("_Rows", vm);
    }
}