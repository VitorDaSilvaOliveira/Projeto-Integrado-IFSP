using Estoque.Infrastructure.Services;
using JJMasterData.Core.UI.Components;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Estoque.Tests.UnitTests
{
    public class ClienteServiceTests
    {
        private readonly Mock<IComponentFactory> _mockFactory;
        private readonly ClienteService _service;

        public ClienteServiceTests()
        {
            _mockFactory = new Mock<IComponentFactory>();
            _service = new ClienteService(_mockFactory.Object);
        }

        [Fact]
        public async Task GetFormViewClienteAsync_Should_Return_Configured_View()
        {
            // Arrange
            var view = (JJFormView)FormatterServices.GetUninitializedObject(typeof(JJFormView));
            
            // Corrigido: CreateAsync apenas com "Cliente"
            _mockFactory.Setup(x => x.FormView.CreateAsync("Cliente"))
                        .ReturnsAsync(view);

            // Act
            var result = await _service.GetFormViewClienteAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ShowTitle);
        }

        [Fact]
        public async Task GetFormViewReportClienteAsync_Should_Return_Configured_View()
        {
            // Arrange
            var view = (JJFormView)FormatterServices.GetUninitializedObject(typeof(JJFormView));
            
            // Corrigido: CreateAsync apenas com "RelatorioCliente"
            _mockFactory.Setup(x => x.FormView.CreateAsync("RelatorioCliente"))
                        .ReturnsAsync(view);

            // Act
            var result = await _service.GetFormViewReportClienteAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ShowTitle);
        }
    }
}