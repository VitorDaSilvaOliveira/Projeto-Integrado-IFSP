using JJMasterData.Web.Configuration;

namespace Estoque.Web.Extensions;

public static class ApplicationBuilderExtensions
{
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
    
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=Login}");
    }
}