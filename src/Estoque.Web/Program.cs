using Estoque.Infrastructure.Services;
using Estoque.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddEstoqueConfiguration();
builder.Services.AddHostedService<PedidoListenerService>();

var app = builder.Build();

app.UseEstoqueWeb();

app.Run();