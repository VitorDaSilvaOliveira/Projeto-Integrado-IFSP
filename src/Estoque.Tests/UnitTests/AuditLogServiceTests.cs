using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

namespace Estoque.Tests.UnitTests
{
    public class AuditLogServiceTests
    {
        private readonly EstoqueDbContext _context;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IComponentFactory> _mockFactory;
        private readonly AuditLogService _service;

        public AuditLogServiceTests()
        {
            // 1. Banco em Memória
            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new EstoqueDbContext(options);

            // 2. Mocks
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockFactory = new Mock<IComponentFactory>();

            // 3. Configurar Contexto HTTP Falso (Para simular usuário logado e IP)
            var context = new DefaultHttpContext();
            
            // Simula Claims do Usuário
            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.NameIdentifier, "user-id-123"),
                new Claim(ClaimTypes.Name, "UsuarioTeste")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            context.User = new ClaimsPrincipal(identity);
            
            // Simula IP
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            // 4. Instancia o Serviço
            _service = new AuditLogService(_context, _mockHttpContextAccessor.Object, _mockFactory.Object);
        }

        [Fact]
        public async Task LogAsync_Should_Save_Log_To_Database()
        {
            // Arrange
            string area = "Produtos";
            string action = "Criar";
            string details = "Produto X criado com sucesso";

            // Act
            await _service.LogAsync(area, action, details);

            // Assert
            var log = await _context.AuditLogs.FirstOrDefaultAsync();
            
            Assert.NotNull(log);
            Assert.Equal("user-id-123", log.UserId);
            Assert.Equal("UsuarioTeste", log.UserName);
            Assert.Equal("127.0.0.1", log.IpAddress);
            Assert.Equal(area, log.Area);
            Assert.Equal(action, log.Action);
            Assert.Equal(details, log.Details);
            
            // Verifica se a data foi preenchida (Recente)
            Assert.True(log.AccessedAt > DateTime.MinValue);
        }
    }
}