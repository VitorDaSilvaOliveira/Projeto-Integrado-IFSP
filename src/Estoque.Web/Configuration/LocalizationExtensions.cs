using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace Estoque.Web.Configuration;

public static class LocalizationExtensions
{
    public static void AddPtBrLocalization(this IServiceCollection services)
    {
        var supportedCultures = new[] { new CultureInfo("pt-BR") };

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("pt-BR");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });
    }

    public static void UsePtBrLocalization(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(options);
    }
}