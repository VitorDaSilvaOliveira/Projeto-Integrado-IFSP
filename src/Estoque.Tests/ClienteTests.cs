using Xunit;
using Estoque.Domain.Entities;
using System.Linq; // Adicione este using

public class ClienteTests
{


    [Theory]
    [InlineData("12345678901")]       // CPF válido sem formatação
    [InlineData("123.456.789-01")]    // CPF válido com formatação
    [InlineData("12345678000190")]    // CNPJ válido sem formatação
    [InlineData("12.345.678/0001-90")] // CNPJ válido com formatação
    public void ValidarDocumento_ComFormatosValidos_DeveRetornarTrue(string documentoValido)
    {
        var cliente = new Cliente();

        // Act
        bool resultado = cliente.ValidarDocumento(documentoValido);

        // Assert
        Assert.True(resultado, $"Documento '{documentoValido}' deveria ser considerado válido.");
    }

    [Theory]
    [InlineData("123")]               // Inválido (muito curto)
    [InlineData("11111111111")]       // Inválido (dígitos repetidos)
    [InlineData("abcdefghijk")]       // Inválido (contém letras)
    [InlineData(null)]                // Inválido (nulo)
    [InlineData("")]                  // Inválido (vazio)
    [InlineData(" ")]                 // Inválido (espaço em branco)
    public void ValidarDocumento_ComFormatosInvalidos_DeveRetornarFalse(string documentoInvalido)
    {
        var cliente = new Cliente();

        bool resultado = cliente.ValidarDocumento(documentoInvalido);

        Assert.False(resultado, $"Documento '{documentoInvalido}' deveria ser considerado inválido.");
    }
}