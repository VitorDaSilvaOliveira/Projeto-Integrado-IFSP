using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using FluentAssertions;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Estoque.Tests.UnitTests;

public class AuditLogServiceTests : IDisposable
{
    private readonly EstoqueDbContext _context;
    private readonly AuditLogService _service;

    public AuditLogServiceTests()
    {
        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new EstoqueDbContext(options);

        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        contextAccessorMock.Setup(c => c.HttpContext).Returns(new DefaultHttpContext());

        _service = new AuditLogService(
            _context,
            contextAccessorMock.Object,
            Mock.Of<IComponentFactory>()
        );
    }

    [Fact]
    public async Task LogAsync_DeveSalvarRegistroNoBanco()
    {
        // Arrange
        string area = "Teste";
        string action = "Criar";
        string details = "Detalhes do teste";
        string user = "admin";

        // Act
        await _service.LogAsync(area, action, details, user, "Admin User");

        // Assert
        var log = await _context.AuditLogs.FirstOrDefaultAsync();
        log.Should().NotBeNull();
        log!.Area.Should().Be(area);
        log.Action.Should().Be(action);
        log.Details.Should().Be(details);
        log.UserId.Should().Be(user);
        
        log.AccessedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
    }

    public void Dispose() => _context.Dispose();
}