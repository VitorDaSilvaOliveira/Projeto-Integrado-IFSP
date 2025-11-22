using Estoque.Infrastructure.Services;
using JJMasterData.Core.UI.Components;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Estoque.Tests.UnitTests
{
    public class FornecedorServiceTests
    {
        private readonly Mock<IComponentFactory> _mockFactory;
        private readonly FornecedorService _service;

        public FornecedorServiceTests()
        {
            _mockFactory = new Mock<IComponentFactory>();
            _service = new FornecedorService(_mockFactory.Object);
        }

        [Fact]
        public async Task GetFormViewFornecedorAsync_Should_Return_Configured_View()
        {
            // Arrange
            var view = (JJFormView)FormatterServices.GetUninitializedObject(typeof(JJFormView));

            _mockFactory.Setup(x => x.FormView.CreateAsync("Fornecedores"))
                        .ReturnsAsync(view);

            // Act
            var result = await _service.GetFormViewFornecedorAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ShowTitle);
            _mockFactory.Verify(x => x.FormView.CreateAsync("Fornecedores"), Times.Once);
        }
    }
}