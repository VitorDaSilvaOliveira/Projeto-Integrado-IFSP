using Estoque.Domain.Entities;
using Estoque.Infrastructure.Factory;
using Estoque.Infrastructure.Services;
using JJMasterData.Commons.Configuration;
using JJMasterData.Commons.Logging;
using JJMasterData.Core.Configuration;
using JJMasterData.Web.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Estoque.Web.Configuration;

public static class ServiceCollectionExtensions
{
    private static readonly CultureInfo[] SupportedCultures =
    {
        new("pt-BR"),
        new("en-US")
    };

    public static void AddEstoqueServices(this IServiceCollection services)
    {
        services.AddScoped<ProdutoService>();
        services.AddScoped<MovimentacaoService>();
        services.AddScoped<FornecedorService>();
        services.AddScoped<CategoriaService>();
        services.AddScoped<LogService>();
        services.AddScoped<AuthService>();
        services.AddScoped<UserService>();
        services.AddScoped<AuditLogService>();
        services.AddScoped<HomeService>();
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

        services.AddLocalization();
    }

    public static void AddLocalization(this IServiceCollection services)
    {
        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("pt-BR");
            options.SupportedCultures = SupportedCultures;
            options.SupportedUICultures = SupportedCultures;

            options.RequestCultureProviders = new IRequestCultureProvider[]
            {
                new CookieRequestCultureProvider(),
                new AcceptLanguageHeaderRequestCultureProvider()
            };
        });
    }

    public static void ConfigureCookieAndSession(this IServiceCollection services, bool isDevelopment)
    {
        var sameSiteModeForCookies = isDevelopment ? SameSiteMode.None : SameSiteMode.Strict;
        const CookieSecurePolicy securePolicyForCookies = CookieSecurePolicy.Always;

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = sameSiteModeForCookies;
            options.Cookie.SecurePolicy = securePolicyForCookies;
            options.Cookie.HttpOnly = true;
            options.LoginPath = "/Identity/SignIn/Index";
            options.AccessDeniedPath = "/Identity/SignIn/AccessDenied";
        });

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.SameSite = sameSiteModeForCookies;
                options.Cookie.SecurePolicy = securePolicyForCookies;
                options.Cookie.HttpOnly = true;
            });

        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = securePolicyForCookies;
            options.Cookie.HttpOnly = true;
            options.IdleTimeout = TimeSpan.FromMinutes(30);
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.None;
            options.Secure = CookieSecurePolicy.Always;
        });
    }
}