using Xunit;
using Estoque.Domain.Entities;  
using Estoque.Domain.Services;  
using System;

public class ProdutoServiceTests
{
    private readonly ProdutoService _produtoService;

    public ProdutoServiceTests()
    {
        _produtoService = new ProdutoService();
    }

    // --- TESTE 1: Teste de Unidade - Caminho Feliz
    [Fact]
    public void CriarNovoProduto_ComDadosValidos_DeveRetornarInstanciaDeProduto()
    {
        var nome = "Notebook Gamer Dell G15";
        var descricao = "i7, 16GB RAM, RTX 3060";
        var preco = 7500.50m;
        var quantidade = 15;

        var produtoCriado = _produtoService.CriarNovoProduto(nome, descricao, preco, quantidade);

        Assert.NotNull(produtoCriado); 
        Assert.Equal(nome, produtoCriado.Nome);
        Assert.Equal(preco, produtoCriado.Preco);
        Assert.Equal(quantidade, produtoCriado.QuantidadeEstoque);
        Assert.IsType<Produto>(produtoCriado); 
    }

    // --- TESTE 2 : Teste de Unidade - Preço Inválido
    [Fact]
    public void CriarNovoProduto_ComPrecoNegativo_DeveLancarArgumentException()
    {
        var nome = "Cadeira Gamer";
        var descricao = "Cadeira ergonômica";
        var precoInvalido = -200m;
        var quantidade = 10;

        var exception = Assert.Throws<ArgumentException>(() =>
            _produtoService.CriarNovoProduto(nome, descricao, precoInvalido, quantidade)
        );

        Assert.Equal("O preço do produto não pode ser negativo.", exception.Message);
    }

    // --- TESTE 3: Teste de Unidade - Valor Limite 
    [Fact]
    public void CriarNovoProduto_ComQuantidadeZero_DeveSerPermitido()
    {
        var nome = "Webcam Logitech C920";
        var descricao = "Full HD 1080p";
        var preco = 450.00m;
        var quantidadeLimite = 0; 

        var produtoCriado = _produtoService.CriarNovoProduto(nome, descricao, preco, quantidadeLimite);

        Assert.NotNull(produtoCriado);
        Assert.Equal(quantidadeLimite, produtoCriado.QuantidadeEstoque);
    }
}