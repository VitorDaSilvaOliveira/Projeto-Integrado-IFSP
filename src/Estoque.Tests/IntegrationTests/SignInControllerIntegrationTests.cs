using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Domain.Models;
using Estoque.Infrastructure.Services;
using Estoque.Web.Areas.Identity.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using Xunit;

public class SignInControllerIntegrationTests
{
    private readonly Mock<AuthService> _authServiceMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<EmailSender> _emailSenderMock;

    public SignInControllerIntegrationTests()
    {
        var userStoreMock = Mock.Of<IUserStore<ApplicationUser>>();

        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock, null, null, null, null, null, null, null, null
        );

        _authServiceMock = new Mock<AuthService>(Mock.Of<IHttpContextAccessor>(), _userManagerMock.Object);

        _emailSenderMock = new Mock<EmailSender>("smtp.server", 587, "user", "password");
    }

    private SignInController CreateController()
    {
        var controller = new SignInController(_authServiceMock.Object, _userManagerMock.Object);

        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // TempData
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;

        // URL Helper (necessário para ForgotPassword)
        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns("https://localhost/reset-password");

        controller.Url = urlHelperMock.Object;

        return controller;
    }

    private ApplicationUser CreateUser()
    {
        return new ApplicationUser("João", "Silva")
        {
            Id = "user123",
            UserName = "teste@teste.com",
            Email = "teste@teste.com",
            Status = UserStatus.Ativo
        };
    }

    // -------------------------------------------------------------
    // GET /SignIn
    // -------------------------------------------------------------
    [Fact]
    public void Index_Get_ReturnsView()
    {
        var controller = CreateController();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }

    // -------------------------------------------------------------
    // POST /SignIn (LOGIN SUCESSO)
    // -------------------------------------------------------------
    [Fact]
    public async Task Index_Post_SuccessfulLogin_RedirectsToHome()
    {
        var user = CreateUser();

        _userManagerMock.Setup(m => m.FindByNameAsync(user.UserName)).ReturnsAsync(user);
        _authServiceMock.Setup(a => a.SignInAsync(user.UserName, "123", false))
            .ReturnsAsync((true, null));

        var controller = CreateController();

        var model = new LoginViewModel
        {
            Login = user.UserName,
            Senha = "123",
            LembrarMe = false
        };

        var result = await controller.Index(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
        Assert.Equal("Estoque", redirect.RouteValues["area"]);
    }

    // -------------------------------------------------------------
    // POST /SignIn (LOGIN FALHA)
    // -------------------------------------------------------------
    [Fact]
    public async Task Index_Post_WrongPassword_ReturnsViewWithError()
    {
        var user = CreateUser();

        _userManagerMock.Setup(m => m.FindByNameAsync(user.UserName)).ReturnsAsync(user);
        _authServiceMock.Setup(a => a.SignInAsync(user.UserName, "errado", false))
            .ReturnsAsync((false, "Senha inválida"));

        var controller = CreateController();

        var model = new LoginViewModel
        {
            Login = user.UserName,
            Senha = "errado",
            LembrarMe = false
        };

        var result = await controller.Index(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.True(controller.ModelState.ContainsKey(string.Empty));
    }

    // -------------------------------------------------------------
    // POST /SignIn (USUÁRIO INATIVO)
    // -------------------------------------------------------------
    [Fact]
    public async Task Index_Post_InactiveUser_ReturnsViewWithError()
    {
        var inactiveUser = CreateUser();
        inactiveUser.Status = UserStatus.Inativo;

        _userManagerMock.Setup(m => m.FindByNameAsync(inactiveUser.UserName))
            .ReturnsAsync(inactiveUser);

        var controller = CreateController();

        var model = new LoginViewModel
        {
            Login = inactiveUser.UserName,
            Senha = "123",
            LembrarMe = false
        };

        var result = await controller.Index(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.True(controller.ModelState.ContainsKey(string.Empty));
    }

    // -------------------------------------------------------------
    // GET /ForgotPassword
    // -------------------------------------------------------------
    [Fact]
    public void ForgotPassword_Get_ReturnsView()
    {
        var controller = CreateController();

        var result = controller.ForgotPassword();

        Assert.IsType<ViewResult>(result);
    }

    // -------------------------------------------------------------
    // POST /ForgotPassword
    // -------------------------------------------------------------
    [Fact]
    public async Task ForgotPassword_Post_SendsEmail()
    {
        var user = CreateUser();

        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token123");
        _emailSenderMock.Setup(e => e.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var controller = CreateController();

        var model = new ForgotPasswordViewModel { Email = user.Email };

        var result = await controller.ForgotPassword(model, _emailSenderMock.Object);

        Assert.IsType<ViewResult>(result);
        Assert.True(controller.TempData.ContainsKey("Success"));
    }

    // -------------------------------------------------------------
    // GET /ResetPassword
    // -------------------------------------------------------------
    [Fact]
    public void ResetPassword_Get_ReturnsViewWithModel()
    {
        var controller = CreateController();

        var result = controller.ResetPassword("tokentest", "email@test.com");

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ResetPasswordViewModel>(view.Model);

        Assert.Equal("tokentest", model.Token);
        Assert.Equal("email@test.com", model.Email);
    }

    // -------------------------------------------------------------
    // POST /ResetPassword (SUCESSO)
    // -------------------------------------------------------------
    [Fact]
    public async Task ResetPassword_Post_Success_ReturnsSuccessView()
    {
        var user = CreateUser();

        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ResetPasswordAsync(user, "token123", "NovaSenha"))
            .ReturnsAsync(IdentityResult.Success);

        var controller = CreateController();

        var model = new ResetPasswordViewModel
        {
            Email = user.Email,
            Token = "token123",
            Password = "NovaSenha",
            ConfirmPassword = "NovaSenha"
        };

        var result = await controller.ResetPassword(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.True(controller.TempData.ContainsKey("Success"));
    }
}
