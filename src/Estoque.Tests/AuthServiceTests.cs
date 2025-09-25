using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.IO;

public class AuthServiceTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthServiceTests()
    {
        var services = new ServiceCollection();
        services.AddLogging(); // Essencial para o UserManager

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        services.AddDbContext<EstoqueDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<EstoqueDbContext>()
            .AddDefaultTokenProviders();

        _serviceProvider = services.BuildServiceProvider();
        _userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    }

    [Fact]
    public async Task ForgotPassword_QuandoUsuarioExistente_DeveGerarTokenDeResetValido()
    {
        var nomeUsuarioExistente = "admin"; 
        
        var user = await _userManager.FindByNameAsync(nomeUsuarioExistente);
        
        Assert.NotNull(user);
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        Assert.False(string.IsNullOrEmpty(token));

        var isTokenValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
        Assert.True(isTokenValid, "O token de reset gerado deveria ser válido.");
    }
}