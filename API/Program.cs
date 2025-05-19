using APIProjIFSP.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Serviços
builder.Services.AddCustomServices();

var app = builder.Build();

// Middlewares
app.UseCors();
app.UseIframeSupport();
app.UseStaticFiles();
app.UseSession();

// Endpoints
app.MapCustomEndpoints();

// Seeding
var logger = app.Services.GetRequiredService<ILogger<Program>>();
await app.UseSeedingAsync(logger);

app.Run();