using JJMasterData.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJJMasterDataWeb();

var app = builder.Build();

app.UseStaticFiles();
app.UseSession();

app.MapDataDictionary();
app.MapMasterData();

await app.UseMasterDataSeedingAsync();

app.Run();
