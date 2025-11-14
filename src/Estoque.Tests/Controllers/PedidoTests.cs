using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using FluentAssertions;
using JJMasterData.Core.Events.Args;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Estoque.Tests.Controllers;

namespace Estoque.Tests.IntegrationTests
{
    public class PedidoFluxoIntegrationTests : IDisposable
    {
        private readonly EstoqueDbContext _context;
        private readonly PedidoService _pedidoService;
        private readonly MovimentacaoService _movimentacaoService;
       
        public PedidoFluxoIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
              .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            _context = new EstoqueDbContext(options);
            var componentFactoryMock = Mock.Of<IComponentFactory>();
            var loggerMock = Mock.Of<ILogger<MovimentacaoService>>();
            var loggerPedidoMock = Mock.Of<ILogger<MovimentacaoService>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "usuarioteste")
            });

            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };
            contextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                contextAccessorMock.Object,
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);

            // ComponentFactory mock (caso precise passar para AuditLog)
            var factoryMock = Mock.Of<IComponentFactory>();

            // AuditLogService real com mocks
            var auditLogService = new AuditLogService(
                _context,
                contextAccessorMock.Object,
                factoryMock
            );

            // MovimentacaoService real com dependências mockadas
            var movimentacaoService = new MovimentacaoService(
                factoryMock,
                _context,
                loggerMock,
                auditLogService,
                signInManagerMock.Object
            );

            // PedidoService com dependências reais e mocks
            _pedidoService = new PedidoService(
                factoryMock,
                _context,
                loggerPedidoMock,
                movimentacaoService
            );
           
        }
        [Fact]
        public async Task ProcessOrder_DeveFinalizarPedidoERegistrarMovimentacao()
        {
            // Arrange
            const int produtoId = 1001;
            const int quantidade = 5;
            const string userId = "usuario-test";

            var produto = new Produto
            {
                IdProduto = produtoId,
                Nome = "Produto Teste",
                Codigo = "1001"
            };

            var cliente = new Cliente { Id = 2001, Nome = "Cliente Teste" };

            var lote = new ProdutoLote
            {
                ProdutoId = produtoId,
                Quantidade = 10,
                QuantidadeDisponivel = 10,
                DataEntrada = DateTime.UtcNow
            };

            var pedido = new Pedido
            {
                Id = 1,
                ClienteId = cliente.Id,
                NumeroPedido = "1",
                Status = PedidoStatus.EmAndamento,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produtoId,
                        Quantidade = quantidade,
                        PrecoVenda = 100.00m,
                        ValorTotal = 500.00m
                    }
                }
            };

            _context.Produtos.Add(produto);
            _context.Clientes.Add(cliente);
            _context.ProdutoLotes.Add(lote);
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            // Act
            await _pedidoService.ProcessOrder(pedido.Id, userId);

            // Assert
            var pedidoAtualizado = await _context.Pedidos.FindAsync(pedido.Id);
            pedidoAtualizado.Status.Should().Be(PedidoStatus.Realizado);

            var loteAtualizado = await _context.ProdutoLotes.FirstAsync(l => l.ProdutoId == produtoId);
            loteAtualizado.QuantidadeDisponivel.Should().Be(5);

            var movimentacao = await _context.Movimentacoes
                .FirstOrDefaultAsync(m => m.IdProduto == produtoId && m.TipoMovimentacao == TipoMovimentacao.Saida);

            movimentacao.Should().NotBeNull();
            movimentacao!.Quantidade.Should().Be(quantidade);
            movimentacao.IdUser.Should().Be(userId);
        }

        [Fact]
        public async Task ProcessOrder_ComEstoqueInsuficiente_DeveLancarExcecao()
        {
            // Arrange
            const int produtoId = 2002;
            const int quantidadeSolicitada = 10;
            const int estoqueDisponivel = 3;
            const string userId = "usuario";

            var produto = new Produto
            {
                IdProduto = produtoId,
                Nome = "Produto Sem Estoque",
                Codigo = "2002"
            };

            var cliente = new Cliente { Id = 3001, Nome = "Cliente Sem Estoque" };

            var lote = new ProdutoLote
            {
                ProdutoId = produtoId,
                Quantidade = estoqueDisponivel,
                QuantidadeDisponivel = estoqueDisponivel,
                DataEntrada = DateTime.UtcNow
            };

            var pedido = new Pedido
            {
                Id = 2,
                ClienteId = cliente.Id,
                NumeroPedido = "2",
                Status = PedidoStatus.EmAndamento,
                Itens = new List<PedidoItem>
        {
            new PedidoItem
            {
                ProdutoId = produtoId,
                Quantidade = quantidadeSolicitada,
                PrecoVenda = 100.00m,
                ValorTotal = 1000.00m
            }
        }
            };

            _context.Produtos.Add(produto);
            _context.Clientes.Add(cliente);
            _context.ProdutoLotes.Add(lote);
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> acao = async () =>
            await _pedidoService.ProcessOrder(pedido.Id, userId);

            // Assert
            await acao.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Estoque insuficiente*");

            await _context.Entry(lote).ReloadAsync();
            await _context.Entry(pedido).ReloadAsync();

            lote.QuantidadeDisponivel.Should().Be(estoqueDisponivel);
            pedido.Status.Should().Be(PedidoStatus.EmAndamento, "status do pedido não deve mudar se a operação falhar");

            // Nenhuma movimentação deve ter sido registrada
            var movimentacao = await _context.Movimentacoes
                .FirstOrDefaultAsync(m => m.IdProduto == produtoId && m.TipoMovimentacao == TipoMovimentacao.Saida);

            movimentacao.Should().BeNull("Não deveria registrar movimentação se o estoque é insuficiente");
        }

       
        public void Dispose()
        {
            _context.Dispose();
        }

       

    }

}
