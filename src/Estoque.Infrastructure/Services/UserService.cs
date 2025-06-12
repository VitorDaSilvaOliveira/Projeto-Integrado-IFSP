using Estoque.Infrastructure.Data;
using JJMasterData.Core.DataDictionary;
using JJMasterData.Core.DataDictionary.Models.Actions;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Hosting;

namespace Estoque.Infrastructure.Services;

public class UserService(EstoqueDbContext context, IWebHostEnvironment env, IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewUserAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("Usuarios");
        formView.ShowTitle = true;

        formView.GridView.ToolbarActions.Add(new UrlRedirectAction
        {
            Name = "Adicionar",
            Icon = IconType.PlusCircle,
            Text = "Adicionar",
            ShowAsButton = true,
            UrlRedirect = "/Admin/User/CreateUser"
        });
        
        formView.GridView.TableActions.Add(new UrlRedirectAction
        {
            Name = "Editar",
            Icon = IconType.Edit,
            UrlRedirect = "/Admin/User/UserDetails?userId={Id}"
        });
        
        return formView;
    }
    
    public byte[]? GetUserAvatarBytes(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return null;

        var avatarFileName = context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.AvatarFileName) 
            .FirstOrDefault();

        if (string.IsNullOrEmpty(avatarFileName))
            return null;

        var avatarPath = Path.Combine(env.WebRootPath, "uploads", "avatars", avatarFileName);

        if (!File.Exists(avatarPath))
            return null;

        return File.ReadAllBytes(avatarPath);
    }
}