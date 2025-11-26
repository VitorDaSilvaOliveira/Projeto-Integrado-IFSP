using System;
using System.Threading.Tasks;
using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Web.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;

namespace Estoque.Tests.IntegrationTests
{
    public class IdentitySeedIntegrationTests
    {
        private readonly ServiceProvider _serviceProvider;

        public IdentitySeedIntegrationTests()
        {
            var services = new ServiceCollection();

            // Banco em memória para Identity
            services.AddDbContext<EstoqueDbContext>(options =>
                options.UseInMemoryDatabase($"IdentitySeedTestDb_{Guid.NewGuid()}"));

            // Registrar Identity real
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<EstoqueDbContext>()
            .AddDefaultTokenProviders();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact(DisplayName = "IdentitySeed deve criar usuário e role Admin corretamente")]
        public async Task IdentitySeed_DeveCriarAdminERole()
        {
            // Act
            await IdentitySeed.CreateAdminAsync(_serviceProvider);

            using var scope = _serviceProvider.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            // Assert role
            var roleExists = await roleManager.RoleExistsAsync("Admin");
            roleExists.Should().BeTrue("a role Admin deve ser criada pelo seed");

            // Assert usuário
            var user = await userManager.FindByNameAsync("Admin");
            user.Should().NotBeNull("o usuário Admin deve ser criado pelo seed");
            user!.Email.Should().Be("admin@gmail.com");
            user.Status.Should().Be(UserStatus.Ativo);

            // Assert usuário está na role
            var isInRole = await userManager.IsInRoleAsync(user, "Admin");
            isInRole.Should().BeTrue("o usuário Admin deve estar na role Admin");
        }

        [Fact(DisplayName = "IdentitySeed deve ser idempotente (rodar duas vezes não cria duplicados)")]
        public async Task IdentitySeed_DeveSerIdempotente()
        {
            // Act 1
            await IdentitySeed.CreateAdminAsync(_serviceProvider);

            // Act 2 (rodar novamente)
            Func<Task> secondRun = () => IdentitySeed.CreateAdminAsync(_serviceProvider);

            // Não deve lançar exceção ao rodar novamente
            await secondRun.Should().NotThrowAsync();

            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            // Assert único usuário admin permanece único
            var adminUser = await userManager.FindByNameAsync("Admin");
            adminUser.Should().NotBeNull();

            // Assert role existente
            var roleExists = await roleManager.RoleExistsAsync("Admin");
            roleExists.Should().BeTrue();
        }
    }
}
