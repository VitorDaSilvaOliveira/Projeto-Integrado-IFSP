using System.Security.Claims;
using Estoque.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Estoque.Infrastructure.Services;

public class AuthService(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory,
    IHttpContextAccessor httpContextAccessor,
    AuditLogService auditLogService)
{
    public async Task<(bool Success, string? ErrorMessage)> SignInAsync(string usernameOrEmail, string password, bool rememberMe)
    {
        var user = await userManager.FindByNameAsync(usernameOrEmail)
                   ?? await userManager.FindByEmailAsync(usernameOrEmail);

        if (user == null)
            return (false, "Tentativa de login inválida.");
    
        if (!await userManager.CheckPasswordAsync(user, password))
            return (false, "Tentativa de login inválida.");

        var principal = await claimsPrincipalFactory.CreateAsync(user);

        await signInManager.SignOutAsync();
        await httpContextAccessor.HttpContext!.SignInAsync(IdentityConstants.ApplicationScheme, principal,
            new AuthenticationProperties { IsPersistent = rememberMe });
    
        await auditLogService.LogAsync("Identity", "Login", "Usuário se logou", user.Id, user.UserName);

        return (true, null);
    }
    
    public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
    {
        return signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
    }

    public async Task<(bool Succeeded, string ErrorMessage)> ExternalLoginSignInAsync()
    {
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return (false, "Erro ao obter informações do login externo.");

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return (false, "Email não encontrado na conta Google.");

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, "Usuário não encontrado. Cadastre-se primeiro.");

        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
            return (true, null);

        var addLoginResult = await userManager.AddLoginAsync(user, info);
        if (!addLoginResult.Succeeded)
            return (false, "Falha ao vincular login externo.");

        await signInManager.SignInAsync(user, isPersistent: false);
        return (true, null);
    }

}