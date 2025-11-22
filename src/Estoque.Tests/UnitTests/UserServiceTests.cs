using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Estoque.Tests.UnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<IComponentFactory> _mockFactory;
        private readonly EstoqueDbContext _context;
        private readonly UserService _service;

        public UserServiceTests()
        {
            // 1. Banco em Memória
            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new EstoqueDbContext(options);

            // 2. Mocks
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockFactory = new Mock<IComponentFactory>(); // Não usamos, mas o construtor pede
            
            _mockEnv.Setup(e => e.WebRootPath).Returns("c:\\wwwroot");

            // 3. Instancia o Serviço
            _service = new UserService(
                _context,
                _mockEnv.Object,
                _mockFactory.Object
            );
        }

        [Fact]
        public async Task GetActiveUsersCount_Should_Return_Only_Active_Users()
        {
            // Arrange
            var user1 = new ApplicationUser("Joao", "Silva") { Id = "1", Status = UserStatus.Ativo };
            var user2 = new ApplicationUser("Maria", "Santos") { Id = "2", Status = UserStatus.Inativo };
            var user3 = new ApplicationUser("Pedro", "Souza") { Id = "3", Status = UserStatus.Ativo };

            _context.Users.AddRange(user1, user2, user3);
            await _context.SaveChangesAsync();

            // Act
            var count = await _service.GetActiveUsersCount();

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public void GetUserAvatarBytes_Should_Return_Null_If_User_Has_No_Avatar()
        {
            // Arrange
            var userId = "user-no-avatar";
            var user = new ApplicationUser("Teste", "User") { Id = userId, AvatarFileName = null };
            
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _service.GetUserAvatarBytes(userId);

            // Assert
            Assert.Null(result);
        }
    }
}