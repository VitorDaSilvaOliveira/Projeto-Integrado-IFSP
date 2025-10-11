using Estoque.Domain.Entities;
using Estoque.Domain.Models;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using Estoque.Web.Areas.Admin.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using JJMasterData.Core.UI.Components;

namespace Estoque.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly UserService _userService;
    private readonly EstoqueDbContext _context;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new EstoqueDbContext(options);

        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null
        );

        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            null, null, null, null
        );

        _userService = new UserService(_context, Mock.Of<IWebHostEnvironment>(), Mock.Of<IComponentFactory>());

        _controller = new UserController(
            _userManagerMock.Object,
            _userService,
            _signInManagerMock.Object
        );
        
        _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
    }

    // --- Testes para UserDetails (GET) ---
    [Fact]
    public async Task UserDetails_DeveRetornarNotFound_SeUsuarioNaoExiste()
    {
        // Arrange
        _userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _controller.UserDetails(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UserDetails_DeveRetornarView_SeUsuarioExiste()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var usuario = new ApplicationUser("Ana", "Silva") { Id = userId.ToString() };
        
        _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(usuario);

        // Act
        var result = await _controller.UserDetails(userId);

        // Assert
        result.Should().BeOfType<ViewResult>();
        result.As<ViewResult>().Model.Should().NotBeNull();
        result.As<ViewResult>().Model.Should().BeOfType<EditUserViewModel>();
    }
    
    // --- Testes para CreateUser (POST) ---

    [Fact]
    public async Task CreateUser_ComModeloInvalido_DeveRetornarViewComErro()
    {
        // Arrange
        var model = new UserViewModel();
        _controller.ModelState.AddModelError("UserName", "O campo UserName é obrigatório");

        // Act
        var result = await _controller.CreateUser(model);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result.As<ViewResult>();
        viewResult.Model.Should().Be(model);
        viewResult.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task CreateUser_FalhaAoCriarUsuario_DeveRetornarViewComErro()
    {
        // Arrange
        var model = new UserViewModel { /* Preencha com dados válidos */ };
        var identityError = new IdentityError { Description = "Erro simulado ao criar usuário." };
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _controller.CreateUser(model);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result.As<ViewResult>();
        viewResult.ViewData.ModelState.ErrorCount.Should().Be(1);
        viewResult.ViewData.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be(identityError.Description);
    }

    [Fact]
    public async Task CreateUser_FalhaAoAdicionarRole_DeveRetornarViewComErro()
    {
        // Arrange
        var model = new UserViewModel { /* Preencha com dados válidos */ };
        var roleError = new IdentityError { Description = "Erro simulado ao adicionar role." };
        
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
            
        _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(roleError));

        // Act
        var result = await _controller.CreateUser(model);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result.As<ViewResult>();
        viewResult.ViewData.ModelState.ErrorCount.Should().Be(1);
        viewResult.ViewData.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be(roleError.Description);
    }

    [Fact]
    public async Task CreateUser_SucessoAoCriar_DeveRedirecionarParaIndex()
    {
        // Arrange
        var model = new UserViewModel
        {
            FirstName = "Teste",
            LastName = "Usuario",
            UserName = "teste.usuario",
            Email = "teste@email.com",
            Password = "Password123!"
        };
        
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
            
        _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.CreateUser(model);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        _controller.TempData["Success"].Should().Be("Usuário criado com sucesso!");
    }
    
    // --- NOVOS TESTES PARA UserDetails (POST) ---

    [Fact]
    public async Task UserDetails_Post_ComModeloInvalido_DeveRetornarView()
    {
        // Arrange
        var model = new EditUserViewModel();
        _controller.ModelState.AddModelError("FirstName", "O campo Nome é obrigatório");
        
        // Act
        var result = await _controller.UserDetails(model);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result.As<ViewResult>();
        viewResult.Model.Should().Be(model);
        viewResult.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task UserDetails_Post_UsuarioNaoEncontrado_DeveRetornarNotFound()
    {
        // Arrange
        var model = new EditUserViewModel { Id = Guid.NewGuid().ToString() };
        _userManagerMock.Setup(m => m.FindByIdAsync(model.Id))
                        .ReturnsAsync((ApplicationUser?)null);
        
        // Act
        var result = await _controller.UserDetails(model);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UserDetails_Post_FalhaAoAtualizar_DeveRetornarViewComErro()
    {
        // Arrange
        var user = new ApplicationUser("Gean", "Bandeira") { Id = Guid.NewGuid().ToString() };
        var model = new EditUserViewModel { Id = user.Id };
        var updateError = new IdentityError { Description = "Falha simulada na atualização." };

        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                        .ReturnsAsync(IdentityResult.Failed(updateError));
        
        // Act
        var result = await _controller.UserDetails(model);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result.As<ViewResult>();
        viewResult.ViewData.ModelState.ErrorCount.Should().Be(1);
        viewResult.ViewData.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be(updateError.Description);
    }

    [Fact]
    public async Task UserDetails_Post_SucessoAoAtualizar_DeveRedirecionar()
    {
        // Arrange
        var user = new ApplicationUser("Gean", "Bandeira") { Id = Guid.NewGuid().ToString() };
        var model = new EditUserViewModel
        {
            Id = user.Id,
            FirstName = "Gean Editado",
            LastName = "Bandeira",
            UserName = "gean.bandeira",
            Email = "gean@teste.com"
        };

        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                        .ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _controller.UserDetails(model);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        var redirectResult = result.As<RedirectToActionResult>();
        redirectResult.ActionName.Should().Be("UserDetails");
        redirectResult.RouteValues!["userId"].Should().Be(user.Id);
        _controller.TempData["Success"].Should().Be("Informações do usuário atualizadas com sucesso!");
    }
}