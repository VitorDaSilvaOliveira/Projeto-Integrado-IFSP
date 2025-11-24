using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using FluentAssertions;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Estoque.Tests.UnitTests;

public class MovimentacaoLogicTests : IDisposable
{
    private readonly EstoqueDbContext _context;
    private readonly MovimentacaoService _service;
    private readonly Mock<AuditLogService> _auditLogMock;

    public MovimentacaoLogicTests()
    {
        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new EstoqueDbContext(options);

        var factoryMock = Mock.Of<IComponentFactory>();
        var loggerMock = Mock.Of<ILogger<MovimentacaoService>>();
        
        // Setup do Identity ignorando nulabilidade para o teste unitário
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new UserManager<ApplicationUser>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        
        var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            userManager, contextAccessor.Object, claimsFactory.Object, null!, null!, null!, null!);

        contextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        _auditLogMock = new Mock<AuditLogService>(_context, contextAccessor.Object, factoryMock);

        _service = new MovimentacaoService(
            factoryMock,
            _context,
            loggerMock,
            _auditLogMock.Object,
            signInManagerMock.Object
        );
    }

    [Fact]
    public async Task Entrada_DeveCriarNovoLote()
    {
        // Arrange
        var produto = new Produto { IdProduto = 10, Nome = "Produto Entrada", Codigo = "E01" };
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        // Act
        await _service.RegistrarMovimentacaoAsync(10, 50, TipoMovimentacao.Entrada, "user1", "Entrada Teste");

        // Assert
        var lote = await _context.ProdutoLotes.FirstOrDefaultAsync(l => l.ProdutoId == 10);
        lote.Should().NotBeNull();
        lote!.Quantidade.Should().Be(50);
        lote.QuantidadeDisponivel.Should().Be(50);
    }

    [Fact]
    public async Task Saida_DeveConsumirLotesNaOrdemCorreta()
    {
        // Arrange
        var produto = new Produto { IdProduto = 20, Nome = "Produto Saida", Codigo = "S01" };
        // Importante: Usar LoteId ao invés de Id
        var lote1 = new ProdutoLote { LoteId = 1, ProdutoId = 20, QuantidadeDisponivel = 10, DataEntrada = DateTime.Now.AddDays(-2) };
        var lote2 = new ProdutoLote { LoteId = 2, ProdutoId = 20, QuantidadeDisponivel = 20, DataEntrada = DateTime.Now.AddDays(-1) };
        
        _context.Produtos.Add(produto);
        _context.ProdutoLotes.AddRange(lote1, lote2);
        await _context.SaveChangesAsync();

        // Act: Retirar 15 (deve zerar o lote1 e tirar 5 do lote2)
        await _service.RegistrarMovimentacaoAsync(20, 15, TipoMovimentacao.Saida, "user1", "Saida Teste");

        // Assert
        var lote1Db = await _context.ProdutoLotes.FindAsync(1);
        var lote2Db = await _context.ProdutoLotes.FindAsync(2);

        lote1Db!.QuantidadeDisponivel.Should().Be(0);
        lote2Db!.QuantidadeDisponivel.Should().Be(15);
    }

    public void Dispose() => _context.Dispose();
}