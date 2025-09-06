using Xunit;
using Estoque.Domain.Entities; 
using System;

public class ProdutoTests
{
    // Teste 1: Caminho feliz da baixa de estoque
    [Fact]
    public void DarBaixaEstoque_ComQuantidadeValida_DeveAtualizarEstoque()
    {
        var produto = new Produto
        {
            Nome = "Mouse Gamer",
            QuantidadeEstoque = 10
        };
        var quantidadeParaBaixa = 3;

        produto.DarBaixaEstoque(quantidadeParaBaixa);

        Assert.Equal(7, produto.QuantidadeEstoque); 
    }

    // Teste 2: Tentativa de deixar o estoque negativo
    [Fact]
    public void DarBaixaEstoque_QuandoQuantidadeInsuficiente_DeveLancarExcecao()
    {
        var produto = new Produto
        {
            Nome = "Teclado Mecânico",
            QuantidadeEstoque = 5 
        };
        var quantidadeParaBaixa = 8; 

        var exception = Assert.Throws<InvalidOperationException>(() =>
            produto.DarBaixaEstoque(quantidadeParaBaixa)
        );

        Assert.Equal("Estoque insuficiente para realizar a baixa.", exception.Message);
    }
    
    // Teste 3: Bônus - Tentar dar baixa de um número negativo
    [Fact]
    public void DarBaixaEstoque_ComQuantidadeNegativa_DeveLancarExcecao()
    {
        var produto = new Produto { QuantidadeEstoque = 20 };
        var quantidadeNegativa = -5;

        var exception = Assert.Throws<ArgumentException>(() => 
            produto.DarBaixaEstoque(quantidadeNegativa)
        );

        Assert.Equal("A quantidade para baixa não pode ser negativa.", exception.Message);
    }
}