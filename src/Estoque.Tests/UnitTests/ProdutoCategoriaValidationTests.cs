using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Estoque.Tests.UnitTests
{
    public class ProdutoCategoriaValidationTests : IDisposable
    {
        private readonly EstoqueDbContext _context;
        private readonly SqliteConnection _connection;

        public ProdutoCategoriaValidationTests()
        {
            // Configurar SQLite in-memory
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "PRAGMA foreign_keys = 1;"; 
                command.ExecuteNonQuery();
            }

            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new EstoqueDbContext(options);
            _context.Database.EnsureCreated();
        }

        [Fact]
        [Trait("Unit", "Produto-Categoria-Validation")]
        public async Task Produto_ComCategoriaInexistente_DeveFalharAoSalvar()
        {
            var produto = new Produto
            {
                IdProduto = 1,
                Nome = "Notebook Dell",
                Preco = 3500.00m,
                Codigo = "NB-001",
                IdCategoria = 999  // Categoria inexistente
            };

            _context.Produtos.Add(produto);

            // ACT & ASSERT
            var act = async () => await _context.SaveChangesAsync();

            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        [Trait("Unit", "Produto-Categoria-Validation")]
        public async Task Produto_ComCategoriaValida_DeveSalvarComSucesso()
        {
            // ARRANGE
            var categoria = new Categoria
            {
                IdCategoria = 1,
                NomeCategoria = "Eletrônicos"
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            var produto = new Produto
            {
                IdProduto = 1,
                Nome = "Notebook Dell",
                Preco = 3500.00m,
                Codigo = "NB-001",
                IdCategoria = 1
            };

            _context.Produtos.Add(produto);

            // ACT
            await _context.SaveChangesAsync();

            // ASSERT
            var produtoSalvo = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.IdProduto == 1);

            produtoSalvo.Should().NotBeNull();
            produtoSalvo.IdCategoria.Should().Be(1);
            produtoSalvo.Categoria.Should().NotBeNull();
            produtoSalvo.Categoria.NomeCategoria.Should().Be("Eletrônicos");
        }

        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
