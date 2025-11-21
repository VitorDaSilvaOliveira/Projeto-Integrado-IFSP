using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Estoque.Tests.UnitTests
{
    public class PedidoServiceTests
    {
        private readonly EstoqueDbContext _context;
        private readonly Mock<IComponentFactory> _mockFactory;
        private readonly Mock<ILogger<MovimentacaoService>> _mockLogger;
        private readonly Mock<MovimentacaoService> _mockMovimentacaoService;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly PedidoService _service;

        public PedidoServiceTests()
        {
            // 1. Configura Banco em Memória
            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new EstoqueDbContext(options);

            // 2. Mocks Básicos
            _mockFactory = new Mock<IComponentFactory>();
            _mockLogger = new Mock<ILogger<MovimentacaoService>>();
            _mockEnv = new Mock<IWebHostEnvironment>();

            // 3. Mock Complexo do MovimentacaoService (AQUI ESTAVA O ERRO)
            // A classe MovimentacaoService tem 5 dependências no construtor:
            // (IComponentFactory, EstoqueDbContext, ILogger, AuditLogService, SignInManager)
            // Passamos null para AuditLogService e SignInManager pois não serão usados pelo método mockado.
            
            var movServiceMock = new Mock<MovimentacaoService>(
                _mockFactory.Object, // 1. Factory
                _context,            // 2. Context
                _mockLogger.Object,  // 3. Logger
                null,                // 4. AuditLogService (Não usado no mock)
                null                 // 5. SignInManager (Não usado no mock)
            );

            // Configuramos o método RegistrarMovimentacaoAsync para não fazer nada (Returns Task.CompletedTask)
            // O 'virtual' que adicionamos antes permite que isso funcione.
            movServiceMock.Setup(x => x.RegistrarMovimentacaoAsync(
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<TipoMovimentacao>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

            _mockMovimentacaoService = movServiceMock;

            // 4. Instancia o PedidoService injetando o Mock
            _service = new PedidoService(
                _mockFactory.Object, 
                _context, 
                _mockLogger.Object, 
                _mockMovimentacaoService.Object, 
                _mockEnv.Object
            );
        }

        [Fact]
        public async Task ProcessOrder_Should_Update_Status_And_Call_Movimentacao()
        {
            // Arrange
            var produto = new Produto { IdProduto = 1, Nome = "Prod A", Preco = 100 };
            var pedido = new Pedido 
            { 
                Id = 1, 
                NumeroPedido = "123", 
                Status = PedidoStatus.EmAndamento,
                Operacao = PedidoOperacao.Vendas
            };
            var item = new PedidoItem 
            { 
                Id = 1, 
                id_Pedido = 1, 
                ProdutoId = 1, 
                Quantidade = 2, 
                PrecoVenda = 100 
            };

            _context.Produtos.Add(produto);
            _context.Pedidos.Add(pedido);
            _context.PedidosItens.Add(item);
            await _context.SaveChangesAsync();

            // Act
            await _service.ProcessOrder(1, "user-teste");

            // Assert
            var pedidoAtualizado = await _context.Pedidos.FindAsync(1);
            
            Assert.NotNull(pedidoAtualizado);
            Assert.Equal(PedidoStatus.Realizado, pedidoAtualizado.Status);
            Assert.Equal(200, pedidoAtualizado.ValorTotal); // 2 * 100

            // Verifica se o serviço de movimentação foi chamado corretamente
            _mockMovimentacaoService.Verify(x => x.RegistrarMovimentacaoAsync(
                1, // IdProduto
                2, // Quantidade
                TipoMovimentacao.Saida, // Tipo
                "user-teste", // User
                It.IsAny<string>() // Obs
            ), Times.Once);
        }

        [Fact]
        public async Task ObterPedidosPorOperacaoAsync_Should_Group_Correctly()
        {
            // Arrange
            _context.Pedidos.AddRange(
                new Pedido { Operacao = PedidoOperacao.Vendas },
                new Pedido { Operacao = PedidoOperacao.Vendas },
                new Pedido { Operacao = null } 
            );
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _service.ObterPedidosPorOperacaoAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado);
        }
    }
}