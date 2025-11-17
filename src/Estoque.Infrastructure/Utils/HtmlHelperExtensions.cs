using Estoque.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Estoque.Infrastructure.Utils;

public static class HtmlHelperExtensions
{
    public static bool HasMenuAccess(this IHtmlHelper? html, string menuId, EstoqueDbContext? db)
    {
        var user = html.ViewContext.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true)
            return false;

        var roleIds = user.Claims
                          .Where(c => c.Type == "RoleId")
                          .Select(c => c.Value)
                          .ToList();

        return db.RoleMenus
                 .Any(rm => roleIds.Contains(rm.RoleId) && rm.MenuId == menuId);
    }
}
