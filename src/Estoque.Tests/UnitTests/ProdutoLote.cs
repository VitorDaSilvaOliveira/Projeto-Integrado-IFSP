using Estoque.Domain.Entities;
using Xunit;
using System;

namespace Estoque.Tests.UnitTests
{
    // #################################################################
    // Testes da Entidade: ProdutoLote (3 Testes)
    // #################################################################
    public class ProdutoLoteTests
    {
        [Fact]
        public void ProdutoLote_Should_Set_And_Get_Properties()
        {
            // Arrange
            var lote = new ProdutoLote
            {
                LoteId = 1,
                ProdutoId = 10,
                FornecedorId = 20,
                Quantidade = 100,
                QuantidadeDisponivel = 50,
                CustoUnitario = 19.99m
            };

            // Assert
            Assert.Equal(1, lote.LoteId);
            Assert.Equal(10, lote.ProdutoId);
            Assert.Equal(20, lote.FornecedorId);
            Assert.Equal(100, lote.Quantidade);
            Assert.Equal(50, lote.QuantidadeDisponivel);
            Assert.Equal(19.99m, lote.CustoUnitario);
        }

        [Fact]
        public void ProdutoLote_Should_Have_Default_DataEntrada_Near_UtcNow()
        {
            // Arrange
            var lote = new ProdutoLote();
            var now = DateTime.UtcNow;

            // Assert
            Assert.True((now - lote.DataEntrada).TotalSeconds < 2);
        }

        [Fact]
        public void ProdutoLote_Should_Initialize_ProdutoSeries_Collection()
        {
            // Arrange
            var lote = new ProdutoLote();
            
            // Assert
            Assert.NotNull(lote.ProdutoSeries);
            Assert.Empty(lote.ProdutoSeries);
        }
    }
}