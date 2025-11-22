using Estoque.Infrastructure.Services;
using JJMasterData.Core.UI.Components;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Estoque.Tests.UnitTests
{
    public class CategoriaServiceTests
    {
        private readonly Mock<IComponentFactory> _mockFactory;
        private readonly CategoriaService _service;

        public CategoriaServiceTests()
        {
            _mockFactory = new Mock<IComponentFactory>();
            _service = new CategoriaService(_mockFactory.Object);
        }

        [Fact]
        public async Task GetFormViewCategoriaAsync_Should_Return_Configured_View()
        {
            // Arrange
            var view = (JJFormView)FormatterServices.GetUninitializedObject(typeof(JJFormView));
            
            _mockFactory.Setup(x => x.FormView.CreateAsync("Categoria"))
                        .ReturnsAsync(view);

            // Act
            var result = await _service.GetFormViewCategoriaAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ShowTitle);
            _mockFactory.Verify(x => x.FormView.CreateAsync("Categoria"), Times.Once);
        }
    }
}