using Estoque.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();

builder.AddEstoqueConfiguration();

var app = builder.Build();

app.UseEstoqueWeb();

app.Run();