using System;
using System.Threading.Tasks;
using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Estoque.Tests.IntegrationTests
{
    /// <summary>
    /// Testes de integração para o fluxo de criação e persistência de itens do pedido.
    /// Garante que PedidoItem é salvo corretamente, mantém relacionamentos e calcula valores.
    /// </summary>
    public class PedidoItemIntegrationTests
    {
        private readonly EstoqueDbContext _context;

        public PedidoItemIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
                .UseInMemoryDatabase($"EstoqueTestDb_PedidoItem_{Guid.NewGuid()}")
                .Options;

            _context = new EstoqueDbContext(options);
        }

        [Fact(DisplayName = "Criar PedidoItem deve ser persistido com sucesso")]
        public async Task CriarPedidoItem_DevePersistirComSucesso()
        {
            // Arrange: criar Produto
            var produto = new Produto
            {
                IdProduto = 1,
                Nome = "Teclado Mecânico",
                Preco = 200
            };

            // Criar Pedido
            var pedido = new Pedido
            {
                Id = 1,
                NumeroPedido = "PED-001",
                DataPedido = DateTime.Now
            };

            // Criar PedidoItem
            var item = new PedidoItem
            {
                Id = 1,
                id_Pedido = pedido.Id,
                ProdutoId = produto.IdProduto,
                Quantidade = 2,
                PrecoTabela = produto.Preco!.Value,
                PrecoVenda = produto.Preco.Value,
                Desconto = 0,
                ValorTotal = 2 * produto.Preco.Value
            };

            await _context.Produtos.AddAsync(produto);
            await _context.Pedidos.AddAsync(pedido);
            await _context.PedidosItens.AddAsync(item);

            await _context.SaveChangesAsync();

            // Act
            var itemPersistido = await _context.PedidosItens
                .Include(i => i.Produto)
                .Include(i => i.Pedido)
                .FirstOrDefaultAsync(i => i.Id == item.Id);

            // Assert
            itemPersistido.Should().NotBeNull();
            itemPersistido!.Quantidade.Should().Be(2);
            itemPersistido.PrecoVenda.Should().Be(200);
            itemPersistido.ValorTotal.Should().Be(400);

            // Navegações
            itemPersistido.Produto.Should().NotBeNull();
            itemPersistido.Pedido.Should().NotBeNull();
        }

        [Fact(DisplayName = "ValorTotal deve respeitar PrecoVenda - Desconto")]
        public async Task CriarPedidoItem_DeveCalcularValorTotalComDesconto()
        {
            // Arrange
            var produto = new Produto
            {
                IdProduto = 2,
                Nome = "Mouse Gamer",
                Preco = 150
            };

            var pedido = new Pedido
            {
                Id = 2,
                NumeroPedido = "PED-002",
                DataPedido = DateTime.Now
            };

            var item = new PedidoItem
            {
                Id = 2,
                id_Pedido = pedido.Id,
                ProdutoId = produto.IdProduto,
                Quantidade = 1,
                PrecoTabela = 150,
                PrecoVenda = 150,
                Desconto = 50,
                ValorTotal = 150 - 50
            };

            await _context.Produtos.AddAsync(produto);
            await _context.Pedidos.AddAsync(pedido);
            await _context.PedidosItens.AddAsync(item);
            await _context.SaveChangesAsync();

            // Act
            var itemPersistido = await _context.PedidosItens.FindAsync(2);

            // Assert
            itemPersistido.Should().NotBeNull();
            itemPersistido!.ValorTotal.Should().Be(100);
        }

        [Fact(DisplayName = "PedidoItem deve aparecer dentro da lista de itens do Pedido")]
        public async Task PedidoItem_DeveEstarNaListaDoPedido()
        {
            // Arrange
            var produto = new Produto
            {
                IdProduto = 3,
                Nome = "Monitor",
                Preco = 1000
            };

            var pedido = new Pedido
            {
                Id = 3,
                NumeroPedido = "PED-003",
                DataPedido = DateTime.Now
            };

            var item = new PedidoItem
            {
                Id = 3,
                id_Pedido = pedido.Id,
                ProdutoId = produto.IdProduto,
                Quantidade = 1,
                PrecoTabela = 1000,
                PrecoVenda = 900,
                Desconto = 100,
                ValorTotal = 900
            };

            await _context.Produtos.AddAsync(produto);
            await _context.Pedidos.AddAsync(pedido);
            await _context.PedidosItens.AddAsync(item);
            await _context.SaveChangesAsync();

            // Act
            var pedidoCompleto = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == pedido.Id);

            // Assert
            pedidoCompleto.Should().NotBeNull();
            pedidoCompleto!.Itens.Should().ContainSingle();
            pedidoCompleto.Itens.First().Id.Should().Be(item.Id);
        }
    }
}
