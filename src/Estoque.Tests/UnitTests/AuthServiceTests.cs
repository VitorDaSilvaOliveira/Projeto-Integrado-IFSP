using Estoque.Domain.Entities;
using Estoque.Domain.Models;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using Estoque.Lib.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Estoque.Tests.UnitTests
{
    public class AuthServiceTests
    {
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignIn;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IUserClaimsPrincipalFactory<ApplicationUser>> _mockClaimsFactory;
        private readonly Mock<IStringLocalizer<EstoqueResources>> _mockLocalizer;
        private readonly Mock<IHttpContextAccessor> _mockAccessor;
        private readonly Mock<AuditLogService> _mockAudit;
        private readonly Mock<IServiceProvider> _mockServiceProvider; // Novo mock necessário
        private readonly Mock<IAuthenticationService> _mockAuthService; // Novo mock necessário
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            // 1. Configura Banco e Audit
            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;
            var context = new EstoqueDbContext(options);
            
            _mockAccessor = new Mock<IHttpContextAccessor>();

            // --- CORREÇÃO DO ERRO DE PROVIDER ---
            // O método SignInAsync precisa de um ServiceProvider que retorne IAuthenticationService
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockAuthService
                .Setup(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask); // Simula sucesso no SignIn do cookie

            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(_mockAuthService.Object);

            var contextHttp = new DefaultHttpContext();
            contextHttp.RequestServices = _mockServiceProvider.Object; // Injeta o provider no contexto
            _mockAccessor.Setup(_ => _.HttpContext).Returns(contextHttp);
            // -------------------------------------

            var factoryMock = new Mock<JJMasterData.Core.UI.Components.IComponentFactory>();
            _mockAudit = new Mock<AuditLogService>(context, _mockAccessor.Object, factoryMock.Object);

            // 2. Mocks do Identity
            _mockUserManager = MockUserManager();
            _mockSignIn = MockSignInManager(_mockUserManager.Object);
            _mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _mockLocalizer = new Mock<IStringLocalizer<EstoqueResources>>();

            // Setup do Localizer
            _mockLocalizer.Setup(l => l[It.IsAny<string>()]).Returns(new LocalizedString("Key", "Mensagem"));

            // Setup do ClaimsFactory
            _mockClaimsFactory.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new ClaimsPrincipal(new ClaimsIdentity()));

            // 3. Instancia o AuthService
            _service = new AuthService(
                _mockSignIn.Object,
                _mockUserManager.Object,
                _mockClaimsFactory.Object,
                _mockLocalizer.Object,
                _mockAccessor.Object,
                _mockAudit.Object
            );
        }

        [Fact]
        public async Task SignInAsync_Should_Return_True_On_Success()
        {
            // Arrange
            var login = "admin";
            var senha = "123";
            var user = new ApplicationUser("Admin", "User") { UserName = login, Id = "1" };

            // 1. Mock encontrar usuário
            _mockUserManager.Setup(x => x.FindByNameAsync(login)).ReturnsAsync(user);
            
            // 2. Mock senha correta
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, senha)).ReturnsAsync(true);

            // 3. Mock SignOut
            _mockSignIn.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);
            
            // Act
            var result = await _service.SignInAsync(login, senha, false);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public async Task SignInAsync_Should_Return_False_If_User_NotFound()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _service.SignInAsync("naoexiste", "123", false);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
        }

        // --- Helpers ---
        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private Mock<SignInManager<ApplicationUser>> MockSignInManager(UserManager<ApplicationUser> userManager)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<ApplicationUser>>();

            return new Mock<SignInManager<ApplicationUser>>(
                userManager,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                logger.Object,
                schemes.Object,
                confirmation.Object
            );
        }
    }
}