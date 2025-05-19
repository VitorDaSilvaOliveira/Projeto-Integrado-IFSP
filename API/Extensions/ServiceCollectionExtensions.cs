using JJMasterData.Web.Configuration;

namespace APIProjIFSP.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddJJMasterDataWeb();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(corsBuilder =>
            {
                corsBuilder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
}