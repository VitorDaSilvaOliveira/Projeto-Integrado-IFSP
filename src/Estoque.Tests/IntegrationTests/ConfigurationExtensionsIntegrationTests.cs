using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using Estoque.Web.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using Xunit;

public class ConfigurationExtensionsIntegrationTests
{
    [Fact]
    public void AddEstoqueConfiguration_Registers_All_Core_Services()
    {
        // Arrange — cria um WebApplicationBuilder real usando ambiente de teste
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });

        // Adiciona ConnectionString falsa (será substituída por InMemory)
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "FakeConnectionString"
        });

        // Substitui SqlServer → InMemory (antes de montar os serviços)
        builder.Services.AddDbContext<EstoqueDbContext>(options =>
            options.UseInMemoryDatabase("EstoqueTestDb")
        );

        // Act — aplica toda a configuração da aplicação
        builder.AddEstoqueConfiguration();

        var provider = builder.Services.BuildServiceProvider();

        // Assert — verifica serviços críticos

        // DbContext
        Assert.NotNull(provider.GetService<EstoqueDbContext>());

        // Identity Core
        Assert.NotNull(provider.GetService<UserManager<ApplicationUser>>());
        Assert.NotNull(provider.GetService<RoleManager<ApplicationRole>>());

        // Auth + Authorization
        Assert.NotNull(provider.GetService<IAuthorizationHandlerProvider>());
        Assert.NotNull(provider.GetService<IAuthenticationService>());

        // Localization
        Assert.NotNull(provider.GetService<IStringLocalizerFactory>());

        // Session
        Assert.NotNull(provider.GetService<ISessionStore>());

        // Serviços do domínio
        Assert.NotNull(provider.GetService<ProdutoService>());
        Assert.NotNull(provider.GetService<ClienteService>());
        Assert.NotNull(provider.GetService<FornecedorService>());
        Assert.NotNull(provider.GetService<PedidoService>());
        Assert.NotNull(provider.GetService<AuthService>());
        Assert.NotNull(provider.GetService<AuditLogService>());
        Assert.NotNull(provider.GetService<NotaFiscalService>());
        Assert.NotNull(provider.GetService<UserService>());

        // Factory de Claims
        Assert.NotNull(provider.GetService<IUserClaimsPrincipalFactory<ApplicationUser>>());
    }
}
