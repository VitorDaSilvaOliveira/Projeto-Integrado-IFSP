using Estoque.Lib;

namespace Estoque.Tests.Controllers;

public class CalculadoraDeDescontoTests
{
    [Fact]
    public void CalcularDesconto_DeveAplicarDescontoCorretamente()
    {
        // Arrange
        var calculadora = new CalculadoraDeDesconto();
        decimal valor = 100;
        decimal percentual = 10;

        // Act
        var resultado = calculadora.CalcularDesconto(valor, percentual);

        // Assert
        Assert.Equal(90, resultado);
    }

    [Fact]
    public void CalcularDesconto_ComValorZero_DeveRetornarZero()
    {
        var calculadora = new CalculadoraDeDesconto();

        var resultado = calculadora.CalcularDesconto(0, 10);

        Assert.Equal(0, resultado);
    }

    [Fact]
    public void CalcularDesconto_Negativo_DeveLancarExcecao()
    {
        var calculadora = new CalculadoraDeDesconto();

        Assert.Throws<ArgumentException>(() => calculadora.CalcularDesconto(-100, 10));
        Assert.Throws<ArgumentException>(() => calculadora.CalcularDesconto(100, -10));
    }
}