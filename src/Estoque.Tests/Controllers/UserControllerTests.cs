using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using Estoque.Web.Areas.Admin.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public UserControllerTests()
    {
        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
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
    }

    [Fact]
    public async Task UserDetails_DeveRetornarNotFound_SeUsuarioNaoExiste()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        var controller = new UserController(
            _userManagerMock.Object,
            _userService,
            _signInManagerMock.Object
        );

        var result = await controller.UserDetails(Guid.NewGuid());

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UserDetails_DeveRetornarView_SeUsuarioExiste()
    {
        var usuario = new ApplicationUser("Ana", "Silva")
        {
            Id = Guid.NewGuid().ToString()
        };

        _userManagerMock.Setup(m => m.FindByIdAsync(usuario.Id.ToString()))
            .ReturnsAsync(usuario);

        var controller = new UserController(
            _userManagerMock.Object,
            _userService,
            _signInManagerMock.Object
        );

        var result = await controller.UserDetails(Guid.Parse(usuario.Id));

        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().NotBeNull();
    }
}