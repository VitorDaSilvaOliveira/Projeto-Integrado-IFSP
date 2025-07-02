using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Factory;
using Estoque.Web.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();

// === Banco de Dados ===
builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("Estoque.Infrastructure")
    )
);

// === Serviços MVC ===
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    options.Filters.Add(new AuthorizeFilter());
});

// === Identity ===
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<ApplicationRole>()
.AddEntityFrameworkStores<EstoqueDbContext>()
.AddErrorDescriber<Estoque.Infrastructure.Services.IdentityErrorDescriberPtBr>()
.AddDefaultTokenProviders();

// === Serviços Customizados ===
builder.Services.AddCustomServices();
builder.Services.AddEstoqueServices();
builder.Services.AddPtBrLocalization();

// === Configuração Cookies & Autenticação ===

var sameSiteModeForCookies = isDevelopment ? SameSiteMode.None : SameSiteMode.Strict;
const CookieSecurePolicy securePolicyForCookies = CookieSecurePolicy.Always; 

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = sameSiteModeForCookies;
    options.Cookie.SecurePolicy = securePolicyForCookies;
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/Identity/SignIn/Index";
    options.AccessDeniedPath = "/Identity/SignIn/AccessDenied";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.SameSite = sameSiteModeForCookies;
        options.Cookie.SecurePolicy = securePolicyForCookies;
        options.Cookie.HttpOnly = true;
    });
    //.AddGoogle(googleOptions =>
    //{
    //    googleOptions.ClientId = builder.Configuration["GoogleKeys:ClientId"];
    //    googleOptions.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
    //    googleOptions.CallbackPath = "/Identity/SignIn/GoogleResponse";

    //    googleOptions.Scope.Add("email");
    //    googleOptions.Scope.Add("profile");
    //    googleOptions.SaveTokens = true;

    //    googleOptions.CorrelationCookie.SameSite = SameSiteMode.None;
    //    googleOptions.CorrelationCookie.SecurePolicy = securePolicyForCookies;
    //});

// === Session ===
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = securePolicyForCookies;
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// === Forwarded Headers (proxies) ===
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// === Autorização ===
builder.Services.AddAuthorization();

// === Política de Cookies ===
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// === Pipeline ===
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Index/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders();

app.UseRouting();
app.UsePtBrLocalization();
app.UseCookiePolicy();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapCustomEndpoints();

app.Run();
