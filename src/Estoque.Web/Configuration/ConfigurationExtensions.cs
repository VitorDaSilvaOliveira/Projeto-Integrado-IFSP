using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Web.Configuration;

public static class ConfigurationExtensions
{
    public static void AddEstoqueConfiguration(this WebApplicationBuilder builder)
    {
        var isDevelopment = builder.Environment.IsDevelopment();

        builder.Services.AddDbContext<EstoqueDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly("Estoque.Infrastructure")
            )
        );

        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            options.Filters.Add(new AuthorizeFilter());
        });

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<EstoqueDbContext>()
        .AddErrorDescriber<Infrastructure.Services.IdentityErrorDescriberPtBr>()
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
}