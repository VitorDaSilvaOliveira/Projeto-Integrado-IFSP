using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using JJMasterData.Core.UI.Components;

namespace Estoque.Tests.IntegrationTests
{
    public class NotaFiscalAzureTests : IDisposable
    {
        private readonly EstoqueDbContext _context;
        private readonly NotaFiscalService _notaFiscalService;
        private const string ConnectionString = "Server=tcp:estoqueifsp.database.windows.net,1433;Initial Catalog=Estoque;Persist Security Info=False;User ID=estoqueifsp;Password=ifspestoque123@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public NotaFiscalAzureTests()
        {
            var options = new DbContextOptionsBuilder<EstoqueDbContext>()
                .UseSqlServer(ConnectionString) // Conecta ao Azure SQL
                .Options;

            _context = new EstoqueDbContext(options);

            try
            {
                _context.Database.ExecuteSqlRaw("TRUNCATE TABLE PedidosItens; TRUNCATE TABLE Pedidos; DELETE FROM Clientes; DBCC CHECKIDENT ('Clientes', RESEED, 0);");
            }
            catch {  }

            var factoryMock = Mock.Of<IComponentFactory>();
            _notaFiscalService = new NotaFiscalService(_context, factoryMock);
        }

        [Fact]
        public async Task GenerateDanfe_ComDadosInjetadosNoAzure_DeveGerarPDFValido()
        {

            //  Criar entidades pai (Cliente e Produto) sem IDs explícitos
            var novoCliente = new Cliente { Nome = "Cliente Azure Test", Documento = "12345678" };
            var novoProduto = new Produto { Nome = "Produto NF Teste", Codigo = "NF_TESTE", Preco = 100.00m };

            _context.Clientes.Add(novoCliente);
            _context.Produtos.Add(novoProduto);

            await _context.SaveChangesAsync();

            int clienteId = novoCliente.Id;
            int produtoId = novoProduto.IdProduto;

            var novoPedido = new Pedido
            {
                ClienteId = clienteId,
                NumeroPedido = "AZURE001",
                Status = Estoque.Domain.Enums.PedidoStatus.Realizado,
                DataPedido = DateTime.Now, // Resolve o erro de DATETIME NULL
                FormaPagamento = 1         // Resolve o erro de INT NULL
            };
            _context.Pedidos.Add(novoPedido);

            await _context.SaveChangesAsync();

            int pedidoId = novoPedido.Id;

            _context.PedidosItens.Add(new PedidoItem
            {
                id_Pedido = pedidoId,
                ProdutoId = produtoId,
                Quantidade = 1,
                PrecoVenda = 100.00m,
                ValorTotal = 100.00m,
                Desconto = 0.00m,     
                PrecoTabela = 100.00m 
            });

            await _context.SaveChangesAsync();

            using var ms = new MemoryStream();

            // Act
            _notaFiscalService.GenerateDanfe(pedidoId, ms);
            ms.Position = 0;

            // Assert
            ms.Length.Should().BeGreaterThan(0, "O PDF deve ser gerado pelo QuestPDF.");

            var bytes = ms.ToArray();
            bytes[0].Should().Be(0x25, "O byte inicial deve ser o caractere '%', indicando PDF.");

            var pedidoVerificado = await _context.Pedidos.FindAsync(pedidoId);
            pedidoVerificado.Should().NotBeNull();
        }
        public void Dispose()
        {
            //  Limpar dados após o teste para evitar sujeira
            try
            {
                _context.Database.ExecuteSqlRaw("TRUNCATE TABLE PedidosItens; TRUNCATE TABLE Pedidos; DELETE FROM Clientes;");
                _context.SaveChanges();
            }
            catch {  }
            _context.Dispose();
        }
    }
}