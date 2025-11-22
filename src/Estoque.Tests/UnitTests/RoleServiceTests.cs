using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using System;

namespace Estoque.Tests.UnitTests
{
    public class RoleServiceTests
    {
        private readonly EstoqueDbContext _context;
        private readonly RoleService _service;

        public RoleServiceTests()
        {
            // Banco em memória
            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new EstoqueDbContext(options);

            // Instancia o serviço (Apenas 1 argumento conforme seu arquivo)
            _service = new RoleService(_context);
        }

        [Fact]
        public async Task HasAccessAsync_Should_Return_True_If_RoleMenu_Exists()
        {
            // Arrange
            var roleId = "role-admin";
            var menuId = "menu-produtos";
            
            _context.RoleMenus.Add(new RoleMenu { RoleId = roleId, MenuId = menuId });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.HasAccessAsync(roleId, menuId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task HasAccessAsync_Should_Return_False_If_RoleMenu_Does_Not_Exist()
        {
            // Arrange
            var roleId = "role-guest";
            var menuId = "menu-financeiro";

            // Act
            var result = await _service.HasAccessAsync(roleId, menuId);

            // Assert
            Assert.False(result);
        }
    }
}