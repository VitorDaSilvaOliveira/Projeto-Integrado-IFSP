using Microsoft.AspNetCore.Identity;

namespace Estoque.Infrastructure.Services;

public class AuthService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
{
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public async Task<(bool Success, string? ErrorMessage)> SignInAsync(string usernameOrEmail, string password, bool rememberMe)
    {
        var user = await _userManager.FindByNameAsync(usernameOrEmail)
                   ?? await _userManager.FindByEmailAsync(usernameOrEmail);

        if (user == null)
            return (false, "Tentativa de login inválida.");

        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
            return (true, null);

        if (result.IsLockedOut)
            return (false, "O usuário está bloqueado.");

        if (result.IsNotAllowed)
            return (false, "O usuário não tem permissão para efetuar login.");

        return (false, "Tentativa de login inválida.");
    }
}
