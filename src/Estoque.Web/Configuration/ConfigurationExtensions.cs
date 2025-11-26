using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Factory;
using Estoque.Infrastructure.Services;
using JJMasterData.Commons.Configuration;
using JJMasterData.Commons.Logging;
using JJMasterData.Core.Configuration;
using JJMasterData.Web.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Estoque.Infrastructure.Repository;
using QuestPDF;
using QuestPDF.Infrastructure;

namespace Estoque.Web.Configuration;

public static class ConfigurationExtensions
{
    public static void AddEstoqueConfiguration(this WebApplicationBuilder builder)
    {
        Settings.License = LicenseType.Community;
        
        var isDevelopment = builder.Environment.IsDevelopment();

        builder.Services.AddDbContext<EstoqueDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly("Estoque.Infrastructure")
            )
        );

        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AuthorizeFilter());
        });

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<EstoqueDbContext>()
        .AddErrorDescriber<IdentityErrorDescriberPtBr>()
        .AddDefaultTokenProviders();

        builder.Services.AddCustomServices();
        builder.Services.AddEstoqueServices();

        builder.Services.ConfigureCookieAndSession(isDevelopment);

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        builder.Services.AddAuthorization();
        builder.Services.AddHttpContextAccessor();
    }

    private static readonly CultureInfo[] SupportedCultures =
    {
        new("pt-BR"),
        new("en-US")
    };

    extension(IServiceCollection services)
    {
        private void AddEstoqueServices()
        {
            services.AddScoped<ProdutoService>();
            services.AddScoped<ClienteService>();
            services.AddScoped<DevolucaoService>();
            services.AddScoped<MovimentacaoService>();
            services.AddScoped<FornecedorService>();
            services.AddScoped<CategoriaService>();
            services.AddScoped<LogService>();
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<AuditLogService>();
            services.AddScoped<HomeService>();
            services.AddScoped<DevolucaoRepository>();
            services.AddSingleton<EmailSender>();
            services.AddScoped<PedidoService>();
            services.AddScoped<RoleService>();
            services.AddScoped<NotaFiscalService>();
            services.AddScoped<PedidoRepository>();
            services.AddScoped<ClienteRepository>();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomClaimsFactory>();
        }

        private void AddCustomServices()
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

        public void AddLocalization()
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("pt-BR");
                options.SupportedCultures = SupportedCultures;
                options.SupportedUICultures = SupportedCultures;

                options.RequestCultureProviders =
                [
                    new CookieRequestCultureProvider(),
                    new AcceptLanguageHeaderRequestCultureProvider()
                ];
            });
        }

        public void ConfigureCookieAndSession(bool isDevelopment)
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
}