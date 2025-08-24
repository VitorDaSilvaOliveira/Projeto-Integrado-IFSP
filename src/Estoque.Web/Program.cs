using Estoque.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddEstoqueConfiguration();

var app = builder.Build();

app.UseEstoqueWeb();

app.Run();