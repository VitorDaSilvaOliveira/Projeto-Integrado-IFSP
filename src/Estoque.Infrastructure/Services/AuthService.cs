using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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

    public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
    {
        return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
    }

    public async Task<(bool Succeeded, string ErrorMessage)> ExternalLoginSignInAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return (false, "Erro ao obter informações do login externo.");

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return (false, "Email não encontrado na conta Google.");

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, "Usuário não encontrado. Cadastre-se primeiro.");

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
            return (true, null);

        var addLoginResult = await _userManager.AddLoginAsync(user, info);
        if (!addLoginResult.Succeeded)
            return (false, "Falha ao vincular login externo.");

        await _signInManager.SignInAsync(user, isPersistent: false);
        return (true, null);
    }

}
