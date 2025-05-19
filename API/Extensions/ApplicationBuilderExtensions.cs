using JJMasterData.Web.Configuration;

namespace APIProjIFSP.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseIframeSupport(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Remove("X-Frame-Options");
            context.Response.Headers.ContentSecurityPolicy = "frame-ancestors *";
            await next();
        });
    }

    public static async Task UseSeedingAsync(this WebApplication app, ILogger logger)
    {
        try
        {
            await app.UseMasterDataSeedingAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro durante o seeding.");
            throw;
        }
    }

    public static void MapCustomEndpoints(this WebApplication app)
    {
        app.MapDataDictionary();
        app.MapMasterData();
    }
}