using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using FluentAssertions;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Estoque.Tests.FlowTests
{
    public class PedidoTests
    {
        private readonly EstoqueDbContext _context;
        private readonly PedidoService _pedidoService;
        private readonly MovimentacaoService _movimentacaoServiceReal;

        public PedidoTests()
        {
            // ARRANGE GLOBAL: Configuração do DB e dos Serviços
            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new EstoqueDbContext(options);

            // Mocks Essenciais
            var loggerMock = Mock.Of<ILogger<MovimentacaoService>>();
            var mockComponentFactory = Mock.Of<IComponentFactory>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            // --- 1. MOCKING DA SEGURANÇA (UserManager e SignInManager) ---
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                null, null, null, null, null, null, null, null
            );

            // Configurar o UserManager para retornar um usuário mockado para GetUserId
            userManagerMock.Setup(u => u.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns("test-user-id");

            // Mocks para as dependências do SignInManager
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var optionsAccessorMock = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
            var loggerSignInManagerMock = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var schemesMock = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
            var userConfirmationMock = new Mock<Microsoft.AspNetCore.Identity.IUserConfirmation<ApplicationUser>>();

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                httpContextAccessorMock.Object,
                userClaimsPrincipalFactoryMock.Object,
                optionsAccessorMock.Object,
                loggerSignInManagerMock.Object,
                schemesMock.Object,
                userConfirmationMock.Object
            );
            var signInManagerObject = signInManagerMock.Object;

            // --- 2. MOCKING DO AUDITLOG SERVICE ---
            var auditLogMock = new Mock<AuditLogService>(
                _context,
                httpContextAccessorMock.Object,
                mockComponentFactory
            );
            var auditLogServiceObject = auditLogMock.Object;

            // --- 3. INSTANCIAÇÃO DO MOVIMENTACAOSERVICE ---
            _movimentacaoServiceReal = new MovimentacaoService(
                mockComponentFactory,
                _context,
                loggerMock,
                auditLogServiceObject,
                signInManagerObject
            );

            // --- 4. INSTANCIAÇÃO DO PEDIDOSERVICE ---
            _pedidoService = new PedidoService(
                mockComponentFactory,
                _context,
                _movimentacaoServiceReal
            );
        }

        [Fact]
        [Trait("E2E", "Fluxo de Finalização de Pedido")]
        public async Task FP_007_FinalizarPedido_DeveBaixarEstoqueCorretamente()
        {
            // ARRANGE
            const int ProdutoId = 10;
            const int LoteId = 20;
            const int ClienteId = 30;
            const int PedidoId = 40;
            const int ItemPedidoId = 50;
            const int FornecedorId = 60;
            const int estoqueInicial = 10;
            const int quantidadePedida = 4;
            const decimal precoVenda = 1500.00m;

            var fornecedor = new Fornecedor { Id = FornecedorId, NomeFantasia = "Fornecedor Teste" };

            var produto = new Produto
            {
                IdProduto = ProdutoId,
                Nome = "Monitor Gamer",
                Preco = precoVenda,
                Codigo = "MG-02"
            };

            var loteInicial = new ProdutoLote
            {
                LoteId = LoteId,
                ProdutoId = ProdutoId,
                FornecedorId = FornecedorId,
                QuantidadeDisponivel = estoqueInicial,
                Quantidade = estoqueInicial,
                CustoUnitario = 500.00m,
                DataEntrada = DateTime.UtcNow
            };

            var cliente = new Cliente
            {
                Id = ClienteId,
                Nome = "Cliente Fulano",
                Documento = "23535838819"
            };

            _context.Fornecedores.Add(fornecedor);
            _context.Produtos.Add(produto);
            _context.Clientes.Add(cliente);
            _context.ProdutoLotes.Add(loteInicial);

            await _context.SaveChangesAsync();

            var pedido = new Pedido
            {
                Id = PedidoId,
                ClienteId = ClienteId,
                DataPedido = DateTime.UtcNow,
                Status = PedidoStatus.EmAndamento,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem {
                        Id = ItemPedidoId,
                        ProdutoId = ProdutoId,
                        Produto = produto,  
                        Quantidade = quantidadePedida,
                        PrecoVenda = precoVenda,
                        ValorTotal = quantidadePedida * precoVenda
                    }
                }
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            // ACT
            var sucesso = await _pedidoService.FinalizarPedidoAsync(pedido.Id);

            // ASSERT
            sucesso.Should().BeTrue("O serviço de finalização deveria retornar sucesso.");

            var pedidoFinalizado = await _context.Pedidos.FindAsync(pedido.Id);
            pedidoFinalizado.Status.Should().Be(PedidoStatus.Finalizado);

            var loteAposBaixa = await _context.ProdutoLotes.FindAsync(LoteId);
            loteAposBaixa.Should().NotBeNull();
            loteAposBaixa.QuantidadeDisponivel.Should().Be(estoqueInicial - quantidadePedida);

            var movimentacao = await _context.Movimentacoes
                .FirstOrDefaultAsync(m => m.IdProduto == ProdutoId &&
                                          m.TipoMovimentacao == TipoMovimentacao.Saida);

            movimentacao.Should().NotBeNull();
            movimentacao.Quantidade.Should().Be(quantidadePedida);
            movimentacao.TipoMovimentacao.Should().Be(TipoMovimentacao.Saida);
            movimentacao.IdProduto.Should().Be(ProdutoId);
        }
    }
}


