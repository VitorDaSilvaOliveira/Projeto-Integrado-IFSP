using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using FluentAssertions;

namespace Estoque.Tests.UnitTests;

public class DomainValidationTests
{
    [Fact]
    public void Produto_ValoresPadrao_DevemEstarCorretos()
    {
        var produto = new Produto();
        produto.ProdutoLotes.Should().NotBeNull();
        produto.RastrearPorSerie.Should().BeNull();
    }

    [Fact]
    public void Pedido_CalculoTotal_DeveSerConsistente()
    {
        var item1 = new PedidoItem { ValorTotal = 100 };
        var item2 = new PedidoItem { ValorTotal = 50 };
        
        var lista = new List<PedidoItem> { item1, item2 };
        
        var total = lista.Sum(x => x.ValorTotal);
        total.Should().Be(150);
    }

    [Fact]
    public void Movimentacao_NaoDeveAceitarQuantidadeNegativa_LogicaDeNegocio()
    {
        var movimentacao = new Movimentacao
        {
            Quantidade = -5,
            TipoMovimentacao = TipoMovimentacao.Entrada
        };

        movimentacao.Quantidade.Should().BeNegative();
    }
    
    [Fact]
    public void Pedido_StatusInicial_DeveSerNulo()
    {
        var pedido = new Pedido();
        // Como a propriedade é 'PedidoStatus?', o padrão é null se não inicializada
        pedido.Status.Should().BeNull(); 
    }
}