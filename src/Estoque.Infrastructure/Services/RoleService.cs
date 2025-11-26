using Estoque.Domain.Entities;
using Estoque.Domain.Models;
using Estoque.Domain.Permissions;
using Estoque.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Services;

public class RoleService(EstoqueDbContext context, RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager)
{
    public async Task<bool> HasAccessAsync(string roleId, string menuId)
    {
        return await context.RoleMenus.AnyAsync(rm => rm.RoleId == roleId && rm.MenuId == menuId);
    }

    public async Task UpdateRoleMenusAsync(string roleId, List<EditMenuGroupViewModel> groups)
    {
        var existing = context.RoleMenus.Where(rm => rm.RoleId == roleId);
        context.RoleMenus.RemoveRange(existing);

        var selectedMenus = groups
            .SelectMany(g => g.Items)
            .Where(i => i.IsSelected)
            .Select(i => new RoleMenu
            {
                RoleId = roleId,
                MenuId = i.MenuId
            })
            .ToList();

        if (selectedMenus.Any())
            await context.RoleMenus.AddRangeAsync(selectedMenus);

        await context.SaveChangesAsync();
    }
    
    public async Task<EditRoleViewModel> BuildViewModelAsync(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId)
                   ?? throw new KeyNotFoundException("Role não encontrada.");

        var allUsers = userManager.Users.ToList();
        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);

        var vmGroups = await BuildGroupsAsync(role);

        return new EditRoleViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name!,
            Groups = vmGroups,
            Users = allUsers.Select(u => new UserRoleViewModel
            {
                UserId = u.Id,
                UserName = u.UserName!,
                IsSelected = usersInRole.Any(x => x.Id == u.Id)
            }).ToList()
        };
    }

    private async Task<List<EditMenuGroupViewModel>> BuildGroupsAsync(ApplicationRole role)
    {
        var definitions = PermissionDefinitions.All;
        var result = new List<EditMenuGroupViewModel>();

        foreach (var g in definitions)
        {
            var gvm = new EditMenuGroupViewModel
            {
                GroupId = g.Id,
                GroupName = g.Name,
                Items = new List<EditMenuViewModel>()
            };

            foreach (var item in g.Items)
            {
                gvm.Items.Add(new EditMenuViewModel
                {
                    MenuId = item.Id,
                    MenuName = item.Name,
                    IsSelected = await HasAccessAsync(role.Id, item.Id)
                });
            }

            result.Add(gvm);
        }

        return result;
    }
}
