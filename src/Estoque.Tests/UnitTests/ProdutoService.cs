using Estoque.Infrastructure.Services;
using JJMasterData.Core.UI.Components;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Runtime.Serialization; // Necessário para o "truque" do objeto

namespace Estoque.Tests.UnitTests
{
    public class ProdutoServiceTests
    {
        private readonly Mock<IComponentFactory> _mockFactory;
        private readonly ProdutoService _service;

        public ProdutoServiceTests()
        {
            _mockFactory = new Mock<IComponentFactory>();
            _service = new ProdutoService(_mockFactory.Object);
        }

        [Fact]
        public async Task GetFormViewProdutoAsync_Should_Create_FormView_And_Set_Title()
        {
            // Arrange
            // TRUQUE: Cria o objeto JJFormView sem chamar o construtor complexo dele
            var formViewReal = (JJFormView)FormatterServices.GetUninitializedObject(typeof(JJFormView));
            
            // Configura o mock para retornar esse objeto "falso"
            // Corrigido: Removido o segundo argumento 'null', passando apenas "Produto"
            _mockFactory.Setup(f => f.FormView.CreateAsync("Produto"))
                        .ReturnsAsync(formViewReal);

            // Act
            var result = await _service.GetFormViewProdutoAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ShowTitle); 
            Assert.Equal(formViewReal, result);
            
            // Verifica se foi chamado com apenas 1 argumento
            _mockFactory.Verify(f => f.FormView.CreateAsync("Produto"), Times.Once);
        }
    }
}