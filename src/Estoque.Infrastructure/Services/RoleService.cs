using Estoque.Domain.Entities;
using Estoque.Domain.Models;
using Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Services;

public class RoleService(EstoqueDbContext context)
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
}
