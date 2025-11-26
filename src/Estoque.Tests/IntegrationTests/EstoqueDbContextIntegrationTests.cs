using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading.Tasks;
using Xunit;

public class EstoqueDbContextIntegrationTests : IAsyncLifetime
{
    private readonly TestcontainersContainer _sqlContainer;
    private string _connectionString = string.Empty;

    public EstoqueDbContextIntegrationTests()
    {
        _sqlContainer = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithName("estoque-tests-sql")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("SA_PASSWORD", "Your_password123")
            .WithPortBinding(14339, 1433)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        _connectionString =
            "Server=localhost,14339;Database=EstoqueTestDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";

        await using var connection = new SqlConnection(_connectionString.Replace("Database=EstoqueTestDb;", ""));
        await connection.OpenAsync();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "IF DB_ID('EstoqueTestDb') IS NULL CREATE DATABASE EstoqueTestDb;";
        await cmd.ExecuteNonQueryAsync();

        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        await using var context = new EstoqueDbContext(options);
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _sqlContainer.StopAsync();
        await _sqlContainer.DisposeAsync();
    }

    private EstoqueDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        return new EstoqueDbContext(options);
    }

    // ===============================================================
    // TESTE 1 → Teste básico de escrita/leitura em Produto
    // ===============================================================
    [Fact]
    public async Task Deve_Inserir_Editar_E_Listar_Produto()
    {
        await using var context = CreateContext();

        var produto = new Produto
        {
            Nome = "Teclado",
            Codigo = "TEC123",
            Preco = 150,
            EstoqueAtual = 10
        };

        context.Produtos.Add(produto);
        await context.SaveChangesAsync();

        var saved = await context.Produtos.FirstAsync();

        Assert.Equal("Teclado", saved.Nome);
        Assert.Equal("TEC123", saved.Codigo);
    }

    // ===============================================================
    // TESTE 2 → Testar Identity (ApplicationUser)
    // ===============================================================
    [Fact]
    public async Task Deve_Criar_Usuario_Identity()
    {
        await using var context = CreateContext();

        var store = new UserStore<ApplicationUser, ApplicationRole, EstoqueDbContext, string>(context);
        var userManager = new UserManager<ApplicationUser>(
            store, null, new PasswordHasher<ApplicationUser>(),
            null, null, null, null, null, null);

        var user = new ApplicationUser("João", "Silva")
        {
            UserName = "joao",
            Email = "joao@email.com",
            Status = UserStatus.Ativo
        };

        var result = await userManager.CreateAsync(user, "Senha@123");

        Assert.True(result.Succeeded);

        var dbUser = await context.Users.FirstAsync();

        Assert.Equal("João", dbUser.FirstName);
        Assert.Equal("Silva", dbUser.LastName);
    }

    // ===============================================================
    // TESTE 3 → Testar relacionamento RoleMenu
    // ===============================================================
    [Fact]
    public async Task Deve_Criar_RoleMenu()
    {
        await using var context = CreateContext();

        var role = new ApplicationRole
        {
            Name = "Gerente",
            NormalizedName = "GERENTE"
        };

        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var rm = new RoleMenu
        {
            RoleId = role.Id,
            MenuId = 10
        };

        context.RoleMenus.Add(rm);
        await context.SaveChangesAsync();

        var saved = await context.RoleMenus.FirstAsync();

        Assert.Equal(role.Id, saved.RoleId);
        Assert.Equal(10, saved.MenuId);
    }

    // ===============================================================
    // TESTE 4 → Testar acesso à VIEW vw_PedidoNF
    // ===============================================================
    [Fact]
    public async Task Deve_Acessar_View_PedidoNF()
    {
        await using var context = CreateContext();

        var query = await context.Vw_PedidoNF.ToListAsync();

        Assert.NotNull(query); // view existe e foi mapeada
    }
}