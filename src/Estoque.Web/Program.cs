using Estoque.Domain.Contexts;
using Estoque.Domain.Models;
using Estoque.Web.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Testando api com SwashBucle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Serviços
builder.Services.AddControllersWithViews();
builder.Services.AddCustomServices();
builder.Services.AddEstoqueServices();
builder.Services.AddPtBrLocalization();
var app = builder.Build();

// Identity
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<Usuario>()
    .AddEntityFrameworkStores<AppDbContext>();

app.MapIdentityApi<Usuario>();

app.UsePtBrLocalization();

// Middlewares
app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();         
app.UseAuthentication();   
app.UseAuthorization();

// Endpoints
app.MapCustomEndpoints();

// Seeding
var logger = app.Services.GetRequiredService<ILogger<Program>>();
await app.UseSeedingAsync(logger);

// Swagger
app.UseSwagger();
app.UseSwaggerUI();
app.MapSwagger();


app.Run();