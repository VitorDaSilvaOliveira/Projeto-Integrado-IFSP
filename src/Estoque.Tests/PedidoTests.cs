using Xunit;
using Estoque.Domain.Entities;
using System.Linq;
using System;

public class PedidoTests
{
    // Teste: Adicionar um item duplicado a um Pedido.
    [Fact]
    public void AdicionarItem_QuandoItemJaExiste_DeveIncrementarQuantidade()
    {
        // Arrange (Preparação)
        var pedido = new Pedido();
        var produto = new Produto { IdProduto = 1, Nome = "Produto Repetido", QuantidadeEstoque = 10 };

        // Adiciona 2 unidades do produto
        pedido.AdicionarItem(new PedidoItem { ProdutoId = produto.IdProduto, Produto = produto, Quantidade = 2 });
        // Adiciona mais 3 unidades do MESMO produto
        pedido.AdicionarItem(new PedidoItem { ProdutoId = produto.IdProduto, Produto = produto, Quantidade = 3 });

        // Assert (Verificação)
        Assert.Single(pedido.Itens); // Deve haver apenas 1 item na lista do pedido
        Assert.Equal(5, pedido.Itens.First().Quantidade); // A quantidade total deve ser 5
    }

    // Teste: Criar um Pedido com um produto sem estoque.
    [Fact]
    public void AdicionarItem_QuandoProdutoSemEstoque_DeveLancarExcecao()
    {
        // Arrange
        var pedido = new Pedido();
        // Criamos um produto em memória com estoque zero para o teste
        var produtoSemEstoque = new Produto { IdProduto = 2, Nome = "Produto Esgotado", QuantidadeEstoque = 0 };
        var itemInvalido = new PedidoItem { ProdutoId = produtoSemEstoque.IdProduto, Produto = produtoSemEstoque, Quantidade = 1 };

        // Act & Assert
        // A ação de adicionar o item deve lançar o erro esperado
        var exception = Assert.Throws<InvalidOperationException>(() =>
            pedido.AdicionarItem(itemInvalido)
        );

        // Verifica a mensagem de erro
        Assert.Equal("Não é possível adicionar um produto sem estoque.", exception.Message);
    }
}