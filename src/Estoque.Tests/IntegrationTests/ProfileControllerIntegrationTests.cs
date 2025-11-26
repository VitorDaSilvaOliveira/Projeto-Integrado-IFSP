using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Domain.Models;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using Estoque.Web.Areas.Identity.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;

public class ProfileControllerIntegrationTests
{
    private readonly EstoqueDbContext _db;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly Mock<UserService> _userServiceMock;

    public ProfileControllerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseInMemoryDatabase("ProfileControllerTestDB")
            .Options;

        _db = new EstoqueDbContext(options);

        var userStore = Mock.Of<IUserStore<ApplicationUser>>();

        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStore, null, null, null, null, null, null, null, null
        );

        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null, null, null, null
        );

        _envMock = new Mock<IWebHostEnvironment>();
        _userServiceMock = new Mock<UserService>(_db);

        // WebRoot temporária
        _envMock.Setup(x => x.WebRootPath).Returns(Path.GetTempPath());
    }

    private ProfileController CreateController(ClaimsPrincipal userPrincipal)
    {
        var controller = new ProfileController(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _envMock.Object,
            _db,
            _userServiceMock.Object
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userPrincipal }
        };

        return controller;
    }

    private ApplicationUser SeedUser()
    {
        var user = new ApplicationUser("João", "Silva")
        {
            Id = "user123",
            Email = "teste@example.com",
            UserName = "teste@example.com",
            Status = UserStatus.Active,
            AvatarFileName = "avatar.png"
        };

        _db.Users.Add(user);
        _db.SaveChanges();
        return user;
    }

    private ClaimsPrincipal CreateUserPrincipal(string userId)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }, "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    // -------------------------------------------------------------------------------------
    // TESTE: GET /Profile/Index
    // -------------------------------------------------------------------------------------
    [Fact]
    public void Index_ReturnsViewWithUserInfo()
    {
        var user = SeedUser();

        _userManagerMock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .Returns(user.Id);

        var controller = CreateController(CreateUserPrincipal(user.Id));

        var result = controller.Index() as ViewResult;

        Assert.NotNull(result);
        Assert.NotNull(result.Model);
    }

    // -------------------------------------------------------------------------------------
    // TESTE: POST /Profile/SavePersonalInformation
    // -------------------------------------------------------------------------------------
    [Fact]
    public async Task SavePersonalInformation_UpdatesUserSuccessfully()
    {
        var user = SeedUser();

        _userManagerMock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .Returns(user.Id);

        var controller = CreateController(CreateUserPrincipal(user.Id));

        var result = await controller.SavePersonalInformation("Carlos", "Santos");

        var updatedUser = _db.Users.First(u => u.Id == user.Id);

        Assert.Equal("Carlos", updatedUser.FirstName);
        Assert.Equal("Santos", updatedUser.LastName);
        Assert.IsType<RedirectToActionResult>(result);
    }

    // -------------------------------------------------------------------------------------
    // TESTE: GET /Profile/Avatar
    // -------------------------------------------------------------------------------------
    [Fact]
    public void Avatar_ReturnsFile_WhenAvatarExists()
    {
        var user = SeedUser();

        _userServiceMock.Setup(s => s.GetUserAvatarBytes(user.Id))
            .Returns(new byte[] { 1, 2, 3 });

        var controller = CreateController(CreateUserPrincipal(user.Id));

        var result = controller.Avatar(user.Id) as FileContentResult;

        Assert.NotNull(result);
        Assert.Equal("image/png", result.ContentType);
    }

    // -------------------------------------------------------------------------------------
    // TESTE: CHANGE EMAIL
    // -------------------------------------------------------------------------------------
    [Fact]
    public async Task ChangeEmail_Post_UpdatesEmailSuccessfully()
    {
        var user = SeedUser();

        var model = new ChangeEmailViewModel
        {
            NewEmail = "novoemail@example.com"
        };

        _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(m => m.GenerateChangeEmailTokenAsync(user, model.NewEmail))
            .ReturnsAsync("token123");

        _userManagerMock.Setup(m => m.ChangeEmailAsync(user, model.NewEmail, "token123"))
            .ReturnsAsync(IdentityResult.Success);

        var controller = CreateController(CreateUserPrincipal(user.Id));

        var result = await controller.ChangeEmail(model);

        Assert.IsType<RedirectToActionResult>(result);
    }
}
