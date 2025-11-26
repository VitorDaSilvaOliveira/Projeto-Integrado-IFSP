using Estoque.Infrastructure.Data;
using Estoque.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Estoque.Tests.Integration;

public class CustomWebApplicationFactoryIntegrationTests
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EstoqueDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<EstoqueDbContext>(options =>
            {
                options.UseInMemoryDatabase("EstoqueTestDb");
            });
        });
    }
}
