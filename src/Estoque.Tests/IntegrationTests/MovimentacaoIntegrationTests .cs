using System;
using System.Linq;
using System.Threading.Tasks;
using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Estoque.Tests.IntegrationTests
{
    public class MovimentacaoIntegrationTests : IAsyncLifetime
    {
        private readonly EstoqueDbContext _context;
        private readonly string _databaseName = $"EstoqueTestDb_{Guid.NewGuid()}";

        public MovimentacaoIntegrationTests()
        {
            // Connection string para SQL Server local ou Azure Dev/Test
            var connectionString = $"Server=(localdb)\\MSSQLLocalDB;Database={_databaseName};Trusted_Connection=True;";

            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging()
                .Options;

            _context = new EstoqueDbContext(options);

            // Cria o banco
            _context.Database.EnsureCreated();
        }

        [Fact(DisplayName = "Criar Movimentacao deve persistir com Produto vinculado")]
        public async Task CriarMovimentacao_DevePersistirComProduto()
        {
            // ARRANGE
            var produto = new Produto
            {
                Nome = "Produto Teste SQL",
                Codigo = "SQL001",
                Preco = 50m,
                EstoqueMinimo = 1
            };

            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();

            var movimentacao = new Movimentacao
            {
                IdProduto = produto.IdProduto,
                Quantidade = 10,
                TipoMovimentacao = TipoMovimentacao.Entrada,
                DataMovimentacao = DateTime.Today,
                Observacao = "Entrada teste SQL Server",
                IdUser = "usuario123"
            };

            // ACT
            await _context.Movimentacoes.AddAsync(movimentacao);
            await _context.SaveChangesAsync();

            // ASSERT
            var movimentacaoDb = await _context.Movimentacoes
                .Include(m => m.Produto)
                .FirstOrDefaultAsync(m => m.IdMovimentacao == movimentacao.IdMovimentacao);

            movimentacaoDb.Should().NotBeNull();
            movimentacaoDb!.Quantidade.Should().Be(10);
            movimentacaoDb.IdProduto.Should().Be(produto.IdProduto);
            movimentacaoDb.Produto.Should().NotBeNull();
            movimentacaoDb.Produto.Nome.Should().Be("Produto Teste SQL");
        }

        // Limpeza do banco após todos os testes
        public async Task InitializeAsync()
        {
            // nada a inicializar além do construtor
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }
    }
}
