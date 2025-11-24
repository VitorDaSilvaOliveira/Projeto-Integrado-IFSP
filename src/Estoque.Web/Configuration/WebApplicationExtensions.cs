using Estoque.Infrastructure.Data;
using JJMasterData.Web.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Estoque.Web.Configuration;

public static class WebApplicationExtensions
{
    public async static void UseEstoqueWeb(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Index/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseForwardedHeaders();

        app.UseRouting();
        app.UseAppLocalization();
        app.UseCookiePolicy();

        app.UseSession();

        app.UseAuthentication();
        app.UseSecurityHeaders();
        app.UseAuthorization();

        app.MapCustomEndpoints();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();

            var retryCount = 5;
            while (retryCount > 0)
            {
                try
                {
                    dbContext.Database.Migrate();
                    break; // OK
                }
                catch (SqlException ex)
                {
                    retryCount--;

                    // Erros transitórios comuns do Azure SQL
                    if (retryCount == 0 || !IsTransientError(ex))
                        throw;

                    await Task.Delay(3000);
                }
            }
        }

        bool IsTransientError(SqlException ex)
        {
            // Lista oficial de erros transitórios do Azure SQL
            return ex.Number == -2  // Timeout
                || ex.Number == 4060
                || ex.Number == 10928
                || ex.Number == 10929
                || ex.Number == 40197
                || ex.Number == 40501
                || ex.Number == 49918
                || ex.Number == 49919
                || ex.Number == 49920;
        }


        IdentitySeed.CreateAdminAsync(app.Services).GetAwaiter().GetResult();
    }

    private static void UseSecurityHeaders(this WebApplication app)
    {
        app.Use((context, next) =>
        {
            context.Response.Headers.Append("Permissions-Policy", "geolocation=(self), microphone=(), camera=()");
            context.Response.Headers.Append("Content-Security-Policy", "frame-ancestors 'self'");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            return next();
        });
    }

    public static void UseAppLocalization(this IApplicationBuilder app)
    {
        var locOptions = app.ApplicationServices
            .GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;

        app.UseRequestLocalization(locOptions);
    }

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
