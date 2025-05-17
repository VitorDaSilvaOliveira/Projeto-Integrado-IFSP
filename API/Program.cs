using JJMasterData.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

// REGISTRA OS SERVIÇOS ANTES DE builder.Build()
builder.Services.AddJJMasterDataWeb();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.UseStaticFiles();
app.UseSession();

// Mapas de rota
app.MapDataDictionary();
app.MapMasterData();

// Seeding com log de erro
try
{
    await app.UseMasterDataSeedingAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Erro durante o seeding: {ex.Message}");
    throw; // rethrow para ver erro completo no Azure
}

app.Run();
