using System.Security.Claims;
using Estoque.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Estoque.Infrastructure.Factory;

public class CustomClaimsFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>(userManager, roleManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("LastName", user.LastName ?? string.Empty));
        identity.AddClaim(new Claim("FirstName", user.FirstName ?? string.Empty));
        return identity;
    }
}
