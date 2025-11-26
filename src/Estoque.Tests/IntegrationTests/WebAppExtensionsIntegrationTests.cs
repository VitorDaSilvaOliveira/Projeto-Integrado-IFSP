using System.Net;
using Estoque.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Estoque.Domain.Entities;
using Xunit;

namespace Estoque.Tests.Integration;

public class WebAppExtensionsIntegrationTests
    : IClassFixture<CustomWebApplicationFactoryIntegrationTests>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactoryIntegrationTests _factory;

    public WebAppExtensionsIntegrationTests(CustomWebApplicationFactoryIntegrationTests factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Application_Should_Start_And_Return_OK_On_Default_Route()
    {
        var response = await _client.GetAsync("/");

        Assert.True(
            response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Redirect,
            "A aplicação não inicializou corretamente."
        );
    }

    [Fact]
    public async Task Application_Should_Apply_Security_Headers()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("Permissions-Policy"));
        Assert.True(response.Headers.Contains("Content-Security-Policy"));
        Assert.True(response.Headers.Contains("Referrer-Policy"));
        Assert.True(response.Headers.Contains("X-Content-Type-Options"));
    }

    [Fact]
    public async Task Database_Should_Apply_Migrations_And_Be_Reachable()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();

        var canConnect = await db.Database.CanConnectAsync();
        Assert.True(canConnect, "O banco InMemory não pôde iniciar.");
    }

    [Fact]
    public async Task IdentitySeed_Should_Create_Admin_User()
    {
        using var scope = _factory.Services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var user = await userManager.FindByNameAsync("Admin");

        Assert.NotNull(user);
        Assert.Equal("ADMIN", user.NormalizedUserName);
        Assert.True(await userManager.IsInRoleAsync(user, "Admin"));
    }

    [Fact]
    public async Task Localization_Should_Be_Applied()
    {
        var response = await _client.GetAsync("/");

        Assert.True(response.Headers.Contains("Set-Cookie"),
            "O middleware de localização não foi aplicado.");
    }

    [Fact]
    public async Task Static_Files_And_Routing_Should_Work()
    {
        var response = await _client.GetAsync("/Identity/SignIn/Index");

        Assert.True(
            response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Redirect,
            "A rota de SignIn não está funcionando corretamente."
        );
    }
}
