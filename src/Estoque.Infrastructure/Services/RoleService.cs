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

    public async Task GrantAccessAsync(string roleId, string menuId)
    {
        if (!await HasAccessAsync(roleId, menuId))
        {
            context.RoleMenus.Add(new RoleMenu { RoleId = roleId, MenuId = menuId });
            await context.SaveChangesAsync();
        }
    }

    public async Task RevokeAccessAsync(string roleId, string menuId)
    {
        var rm = await context.RoleMenus.FirstOrDefaultAsync(r => r.RoleId == roleId && r.MenuId == menuId);
        if (rm != null)
        {
            context.RoleMenus.Remove(rm);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateRoleMenusAsync(string roleId, List<EditMenuViewModel> menus)
    {
        var existing = context.RoleMenus.Where(rm => rm.RoleId == roleId);
        context.RoleMenus.RemoveRange(existing);

        var selectedMenus = menus
            .Where(m => m.IsSelected)
            .Select(m => new RoleMenu
            {
                RoleId = roleId,
                MenuId = m.MenuId
            });

        await context.RoleMenus.AddRangeAsync(selectedMenus);
        await context.SaveChangesAsync();
    }


}
