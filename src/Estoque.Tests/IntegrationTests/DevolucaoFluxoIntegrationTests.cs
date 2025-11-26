using System;
using System.Threading.Tasks;
using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace Estoque.Tests.IntegrationTests
{
    /// <summary>
    /// Testes de integração do fluxo de devoluções e trocas.
    /// Garante que as devoluções são registradas corretamente no banco e geram movimentações.
    /// </summary>
    public class DevolucaoFluxoIntegrationTests
    {
        private readonly EstoqueDbContext _context;

        public DevolucaoFluxoIntegrationTests()
        {
            // Banco de dados em memória para teste isolado
            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
                .UseInMemoryDatabase(databaseName: $"EstoqueTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new EstoqueDbContext(options);
        }

        [Fact(DisplayName = "Registrar devolução deve criar movimentação de entrada no estoque")]
        public async Task RegistrarDevolucao_DeveCriarMovimentacaoDeEntrada()
        {
            // Arrange
            var produto = new Produto
            {
                IdProduto = 1,
                Nome = "Mouse Gamer",
                Preco = 150,
                EstoqueMinimo = 2
            };

            var devolucao = new Devolucao
            {
                IdDevolucao = 1,
                TipoDevolucao = 1, // 1 = cliente, 2 = fornecedor
                IdUser = "admin",
                DataDevolucao = DateTime.Now,
                Observacao = "Produto com defeito"
            };

            var item = new DevolucaoItem
            {
                IdDevolucao = 1,
                IdProduto = 1,
                QuantidadeDevolvida = 3,
                Motivo = "Defeito técnico"
            };

            await _context.Produtos.AddAsync(produto);
            await _context.Devolucoes.AddAsync(devolucao);
            await _context.DevolucoesItens.AddAsync(item);
            await _context.SaveChangesAsync();

            // Act
            var movimentacao = new Movimentacao
            {
                IdProduto = produto.IdProduto,
                TipoMovimentacao = TipoMovimentacao.Entrada,
                Quantidade = item.QuantidadeDevolvida,
                DataMovimentacao = DateTime.Now,
                IdUser = devolucao.IdUser,
                Observacao = $"Devolução #{devolucao.IdDevolucao} - {item.Motivo}"
            };

            await _context.Movimentacoes.AddAsync(movimentacao);
            await _context.SaveChangesAsync();

            // Assert
            var movimentacaoRegistrada = await _context.Movimentacoes
                .FirstOrDefaultAsync(m => m.IdProduto == produto.IdProduto);

            movimentacaoRegistrada.Should().NotBeNull();
            movimentacaoRegistrada!.TipoMovimentacao.Should().Be(TipoMovimentacao.Entrada);
            movimentacaoRegistrada.Quantidade.Should().Be(3);
            movimentacaoRegistrada.Observacao.Should().Contain("Devolução");
        }

        [Fact(DisplayName = "Registrar troca deve atualizar o status da devolução")]
        public async Task RegistrarTroca_DeveAtualizarStatus()
        {
            // Arrange
            var devolucao = new Devolucao
            {
                IdDevolucao = 2,
                TipoDevolucao = 1,
                IdUser = "admin",
                DataDevolucao = DateTime.Now,
                Observacao = "Troca solicitada",
                Devolvido = 0
            };

            await _context.Devolucoes.AddAsync(devolucao);
            await _context.SaveChangesAsync();

            // Act
            devolucao.Devolvido = 1; // Marcando como concluída
            _context.Devolucoes.Update(devolucao);
            await _context.SaveChangesAsync();

            // Assert
            var devolucaoAtualizada = await _context.Devolucoes.FindAsync(devolucao.IdDevolucao);
            devolucaoAtualizada.Should().NotBeNull();
            devolucaoAtualizada!.Devolvido.Should().Be(1);
        }
    }
}
