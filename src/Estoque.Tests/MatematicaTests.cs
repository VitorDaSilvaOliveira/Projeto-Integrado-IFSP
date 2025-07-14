using Xunit;

namespace Estoque.Tests
{
    public class MatematicaTests
    {
        [Fact]
        public void Soma_DeveRetornarResultadoCorreto()
        {
            // Arrange (Preparação)
            int a = 5;
            int b = 7;

            // Act (Ação)
            int resultado = a + b;

            // Assert (Validação)
            Assert.Equal(12, resultado);
        }

        [Fact]
        public void Multiplicacao_DeveRetornarResultadoCorreto()
        {
            int a = 3;
            int b = 4;

            int resultado = a * b;

            Assert.Equal(12, resultado);
        }
    }
}
