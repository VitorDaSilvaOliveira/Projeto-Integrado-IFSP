using JJMasterData.Web.Configuration;

namespace Estoque.Web.Configuration;

public static class ApplicationBuilderExtensions
{
    public static void MapCustomEndpoints(this WebApplication app)
    {
        app.MapDataDictionary();
        app.MapMasterData();

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=SignIn}/{action=Index}/{id?}",
            defaults: new { area = "Identity" });
    }
}