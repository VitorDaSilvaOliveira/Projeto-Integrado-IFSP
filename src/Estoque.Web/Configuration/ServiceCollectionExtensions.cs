using Estoque.Domain.Entities;
using Estoque.Infrastructure.Factory;
using Estoque.Infrastructure.Services;
using JJMasterData.Commons.Configuration;
using JJMasterData.Commons.Logging;
using JJMasterData.Core.Configuration;
using JJMasterData.Web.Configuration;
using Microsoft.AspNetCore.Identity;

namespace Estoque.Web.Configuration;

public static class ServiceCollectionExtensions
{
    public static void AddEstoqueServices(this IServiceCollection services)
    {
        services.AddScoped<ProdutoService>();
        services.AddScoped<MovimentacaoService>();
        services.AddScoped<FornecedorService>();
        services.AddScoped<CategoriaService>();
        services.AddScoped<AuthService>();
        services.AddScoped<UserService>();
        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomClaimsFactory>();
    }

    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddJJMasterDataWeb();
        services.AddJJMasterDataCommons();
        services.AddJJMasterDataCore();
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddDbLoggerProvider();
        });
    }
}