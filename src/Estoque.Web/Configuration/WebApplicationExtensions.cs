using Estoque.Infrastructure.Data;
using JJMasterData.Web.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Estoque.Web.Configuration;

public static class WebApplicationExtensions
{
    public static void UseEstoqueWeb(this WebApplication app)
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
            dbContext.Database.Migrate();
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