using Estoque.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Serviços
builder.Services.AddControllersWithViews();
builder.Services.AddCustomServices();
builder.Services.AddEstoqueServices();

var app = builder.Build();

// Middlewares
app.UseCors();
app.UseIframeSupport();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();          
app.UseAuthentication();   
app.UseAuthorization();

// Endpoints
app.MapCustomEndpoints();

// Seeding
var logger = app.Services.GetRequiredService<ILogger<Program>>();
await app.UseSeedingAsync(logger);

app.Run();