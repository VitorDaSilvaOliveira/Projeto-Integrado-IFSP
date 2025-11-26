using System;
using System.Threading.Tasks;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using Estoque.Web.Areas.Admin.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Estoque.Tests.IntegrationTests
{
    public class AuditLogControllerIntegrationTests
    {
        private readonly EstoqueDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly AuditLogController _controller;

        public AuditLogControllerIntegrationTests()
        {
            // 1. Criar banco em memória isolado
            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
                .UseInMemoryDatabase(databaseName: $"EstoqueTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new EstoqueDbContext(options);

            // (Opcional) Inserir registros mínimos para o serviço funcionar
            // Isso depende do que seu GetFormViewAuditLogAsync espera
            // Aqui vamos deixar sem inserir nada, pois o controller só precisa retornar a view.

            // 2. Instanciar o AuditLogService com o DbContext real
            _auditLogService = new AuditLogService(_context);

            // 3. Instanciar o controller real fornecendo o serviço
            _controller = new AuditLogController(_auditLogService);
        }

        [Fact(DisplayName = "AuditLogController.Index deve retornar ViewResult e carregar ViewBag")]
        public async Task Index_DeveRetornarViewResultEPreencherViewBag()
        {
            // Act
            var result = await _controller.Index();

            // Assert 1 — Deve ser uma View
