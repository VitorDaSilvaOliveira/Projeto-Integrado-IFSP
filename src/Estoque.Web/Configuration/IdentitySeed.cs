using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Estoque.Web.Configuration;

public static class IdentitySeed
{
    public static async Task CreateAdminAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        const string roleName = "Admin";
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new ApplicationRole
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                CreationDate = DateTime.UtcNow
            });
        }

        var userName = "Admin";
        var email = "admin@gmail.com";

        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            user = new ApplicationUser("Admin", "Admin")
            {
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = true,
                Status = UserStatus.Ativo
            };

            var result = await userManager.CreateAsync(user, "Admin@123");
            if (!result.Succeeded)
            {
                var msg = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception("Erro ao criar admin: " + msg);
            }

            await userManager.AddToRoleAsync(user, roleName);
        }
    }
}