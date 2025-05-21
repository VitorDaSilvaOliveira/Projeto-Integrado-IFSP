using Estoque.Infrastructure.Services;
using JJMasterData.Commons.Configuration;
using JJMasterData.Commons.Logging;
using JJMasterData.Core.Configuration;
using JJMasterData.Web.Configuration;

namespace Estoque.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddEstoqueServices(this IServiceCollection services)
    {
        services.AddScoped<ProdutoService>();
        services.AddScoped<FornecedorService>();
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