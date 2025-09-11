using System.Security.Claims;
using Estoque.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Estoque.Lib.Resources;
using Microsoft.Extensions.Localization;
namespace Estoque.Infrastructure.Services;

public class AuthService(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory,
    IStringLocalizer<EstoqueResources> Localizer,
    IHttpContextAccessor httpContextAccessor,
    AuditLogService auditLogService)
{
    public async Task<(bool Success, string? ErrorMessage)> SignInAsync(string usernameOrEmail, string password, bool rememberMe)
    {
        var user = await userManager.FindByNameAsync(usernameOrEmail)
                   ?? await userManager.FindByEmailAsync(usernameOrEmail);

        if (user == null)
            return (false, Localizer["InvalidLoginAttempt"]);

        if (!await userManager.CheckPasswordAsync(user, password))
            return (false, Localizer["InvalidLoginAttempt"]);

        var principal = await claimsPrincipalFactory.CreateAsync(user);

        await signInManager.SignOutAsync();
        await httpContextAccessor.HttpContext!.SignInAsync(IdentityConstants.ApplicationScheme, principal,
            new AuthenticationProperties { IsPersistent = rememberMe });
    
        await auditLogService.LogAsync("Identity", "Login", Localizer["UserLoggedIn"], user.Id, user.UserName);

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
            return (false, Localizer["ExternalLoginInfoError"]);

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return (false, Localizer["EmailNotFound"]);

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, Localizer["UserNotFoundRegisterFirst"]);

        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
            return (true, null);

        var addLoginResult = await userManager.AddLoginAsync(user, info);
        if (!addLoginResult.Succeeded)
            return (false, Localizer["ExternalLoginLinkFailed"]);

        await signInManager.SignInAsync(user, isPersistent: false);
        return (true, null);
    }

}