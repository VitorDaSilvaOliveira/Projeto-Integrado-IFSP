using Estoque.Infrastructure.Data;
using Estoque.Web.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

// === Serviços Customizados ===
builder.Services.AddCustomServices();
builder.Services.AddEstoqueServices();
builder.Services.AddPtBrLocalization();

// === Identity ===
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<EstoqueDbContext>()
.AddDefaultTokenProviders();

// Configura redirecionamento padrão de login
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/SignIn/Index";
    options.AccessDeniedPath = "/Identity/SignIn/AccessDenied";
});

// === Autenticação ===
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["GoogleKeys:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
    googleOptions.CallbackPath = "/Identity/SignIn/GoogleResponse";
    googleOptions.Scope.Add("email");
    googleOptions.Scope.Add("profile");
    googleOptions.SaveTokens = true;
});

// === Autorização ===
builder.Services.AddAuthorization();

var app = builder.Build();

// === Pipeline ===
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Index/Error");
    app.UseHsts();
}

app.UsePtBrLocalization();

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// === Endpoints ===
app.MapCustomEndpoints();

// === Seeding ===
var logger = app.Services.GetRequiredService<ILogger<Program>>();
await app.UseSeedingAsync(logger);

app.Run();
