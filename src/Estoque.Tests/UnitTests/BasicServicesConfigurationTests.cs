using System.Runtime.Serialization;
using Estoque.Infrastructure.Services;
using FluentAssertions;
using JJMasterData.Core.UI.Components;
using Moq;

namespace Estoque.Tests.UnitTests;

public class BasicServicesConfigurationTests
{
    private readonly Mock<IComponentFactory> _factoryMock;

    public BasicServicesConfigurationTests()
    {
        _factoryMock = new Mock<IComponentFactory> { DefaultValue = DefaultValue.Mock };
    }

    // Helper para criar instância sem chamar o construtor complexo
    private JJFormView CreateFakeFormView(string name)
    {
        var view = (JJFormView)FormatterServices.GetUninitializedObject(typeof(JJFormView));
        view.Name = name;
        return view;
    }

    [Fact]
    public async Task ProdutoService_GetFormView_DeveRetornarConfiguracaoCorreta()
    {
        // Arrange
        var expectedView = CreateFakeFormView("Produto");
        
        _factoryMock.Setup(x => x.FormView.CreateAsync("Produto")).ReturnsAsync(expectedView);
        
        var service = new ProdutoService(_factoryMock.Object);

        // Act
        var result = await service.GetFormViewProdutoAsync();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Produto");
        result.ShowTitle.Should().BeTrue();
    }

    [Fact]
    public async Task CategoriaService_GetFormView_DeveRetornarConfiguracaoCorreta()
    {
        // Arrange
        var expectedView = CreateFakeFormView("Categoria");
        _factoryMock.Setup(x => x.FormView.CreateAsync("Categoria")).ReturnsAsync(expectedView);
        
        var service = new CategoriaService(_factoryMock.Object);

        // Act
        var result = await service.GetFormViewCategoriaAsync();

        // Assert
        result.Should().NotBeNull();
        result.ShowTitle.Should().BeTrue();
    }

    [Fact]
    public async Task FornecedorService_GetFormView_DeveRetornarConfiguracaoCorreta()
    {
        // Arrange
        var expectedView = CreateFakeFormView("Fornecedores");
        _factoryMock.Setup(x => x.FormView.CreateAsync("Fornecedores")).ReturnsAsync(expectedView);
        
        var service = new FornecedorService(_factoryMock.Object);

        // Act
        var result = await service.GetFormViewFornecedorAsync();

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Fornecedores");
    }
}