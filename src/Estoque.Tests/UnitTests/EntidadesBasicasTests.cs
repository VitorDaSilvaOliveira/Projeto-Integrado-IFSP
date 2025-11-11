using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Xunit;
using System;

namespace Estoque.Tests.UnitTests
{
    // #################################################################
    // Testes da Entidade: Cliente (17 Testes)
    // #################################################################
    public class ClienteTests
    {
        // Teste 1: Deve definir e obter o Nome
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Nome()
        {
            // Arrange
            var cliente = new Cliente();
            var nome = "Cliente Teste";

            // Act
            cliente.Nome = nome;

            // Assert
            Assert.Equal(nome, cliente.Nome);
        }

        // Teste 2: Deve definir e obter o Documento
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Documento()
        {
            // Arrange
            var cliente = new Cliente();
            var doc = "123.456.789-00";

            // Act
            cliente.Documento = doc;

            // Assert
            Assert.Equal(doc, cliente.Documento);
        }

        // Teste 3: Deve definir e obter o Telefone
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Telefone()
        {
            // Arrange
            var cliente = new Cliente();
            var tel = "(11) 99999-8888";

            // Act
            cliente.Telefone = tel;

            // Assert
            Assert.Equal(tel, cliente.Telefone);
        }

        // Teste 4: Deve definir e obter o Email
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Email()
        {
            // Arrange
            var cliente = new Cliente();
            var email = "teste@cliente.com";

            // Act
            cliente.Email = email;

            // Assert
            Assert.Equal(email, cliente.Email);
        }

        // Teste 5: Deve definir e obter o NomeFantasia
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_NomeFantasia()
        {
            // Arrange
            var cliente = new Cliente();
            var nome = "Empresa Fantasia";

            // Act
            cliente.NomeFantasia = nome;

            // Assert
            Assert.Equal(nome, cliente.NomeFantasia);
        }

        // Teste 6: Propriedades anuláveis devem aceitar null
        [Fact]
        public void ClienteEntity_Should_Allow_Null_Optional_Properties()
        {
            // Arrange
            var cliente = new Cliente();

            // Act
            cliente.Documento = null;
            cliente.Telefone = null;
            cliente.Email = null;
            cliente.NomeFantasia = null;
            cliente.NomeContato = null;
            cliente.Anexo = null;

            // Assert
            Assert.Null(cliente.Documento);
            Assert.Null(cliente.Telefone);
            Assert.Null(cliente.Email);
            Assert.Null(cliente.NomeFantasia);
            Assert.Null(cliente.NomeContato);
            Assert.Null(cliente.Anexo);
        }

        // Teste 7: Deve definir e obter o Tipo de Cliente (Fisica)
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Tipo()
        {
            // Arrange
            var cliente = new Cliente();
            // Corrigido: O Enum é 'Fisica', não 'PessoaFisica'
            var tipo = TipoCliente.Fisica;

            // Act
            cliente.Tipo = tipo;

            // Assert
            Assert.Equal(tipo, cliente.Tipo);
        }

        // --- Testes de Valores Padrão ---

        // Teste 8: Cliente recém-criado deve ter Id 0
        [Fact]
        public void ClienteEntity_Should_Have_Default_Id_Zero()
        {
            // Arrange
            var cliente = new Cliente();
            // Act
            // Assert
            Assert.Equal(0, cliente.Id);
        }

        // Teste 9: Cliente recém-criado deve ter Nome como string vazia
        [Fact]
        public void ClienteEntity_Should_Have_Default_Empty_Nome()
        {
            // Arrange
            var cliente = new Cliente();
            // Act
            // Assert
            // Corrigido: A entidade inicializa Nome = string.Empty
            Assert.Equal(string.Empty, cliente.Nome);
        }

        // Teste 10: Cliente recém-criado deve ter Status padrão (Inativo)
        [Fact]
        public void ClienteEntity_Should_Have_Default_Status_Inativo()
        {
            // Arrange
            var cliente = new Cliente();
            // Act
            // Assert
            // Corrigido: O valor padrão do enum (0) é Inativo
            Assert.Equal(UserStatus.Inativo, cliente.Status);
        }

        // Teste 11: Cliente recém-criado deve ter DataCadastro com valor padrão
        [Fact]
        public void ClienteEntity_Should_Have_Default_DataCadastro()
        {
            // Arrange
            var cliente = new Cliente();
            // Act
            // Assert
            // Corrigido: O valor padrão de DateTime (não anulável) é MinValue
            Assert.Equal(DateTime.MinValue, cliente.DataCadastro);
        }


        // Teste 12: Cliente recém-criado deve ter Tipo padrão (valor 0)
        [Fact]
        public void ClienteEntity_Should_Have_Default_Tipo()
        {
            // Arrange
            var cliente = new Cliente();
            // Act
            // Assert
            // Corrigido: O valor padrão do enum (0) não está definido em TipoCliente (que começa em 1).
            Assert.Equal((TipoCliente)0, cliente.Tipo);
        }

        // --- Testes de Set/Get das propriedades restantes ---

        // Teste 13: Deve definir e obter o Id
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Id()
        {
            // Arrange
            var cliente = new Cliente();
            var id = 100;
            // Act
            cliente.Id = id;
            // Assert
            Assert.Equal(id, cliente.Id);
        }

        // Teste 14: Deve definir e obter o Status
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Status()
        {
            // Arrange
            var cliente = new Cliente();
            var status = UserStatus.Ativo;
            // Act
            cliente.Status = status;
            // Assert
            Assert.Equal(status, cliente.Status);
        }

        // Teste 15: Deve definir e obter a DataCadastro
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_DataCadastro()
        {
            // Arrange
            var cliente = new Cliente();
            var data = DateTime.Now.AddDays(-1);
            // Act
            cliente.DataCadastro = data;
            // Assert
            Assert.Equal(data, cliente.DataCadastro);
        }

        // Teste 16: Deve definir e obter o NomeContato
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_NomeContato()
        {
            // Arrange
            var cliente = new Cliente();
            var nome = "Contato Principal";
            // Act
            cliente.NomeContato = nome;
            // Assert
            Assert.Equal(nome, cliente.NomeContato);
        }

        // Teste 17: Deve definir e obter o Anexo
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Anexo()
        {
            // Arrange
            var cliente = new Cliente();
            var anexo = "caminho/do/anexo.pdf";
            // Act
            cliente.Anexo = anexo;
            // Assert
            Assert.Equal(anexo, cliente.Anexo);
        }
    }

    // #################################################################
    // Testes da Entidade: Pedido (15 Testes)
    // #################################################################
    public class PedidoTests
    {
        // Teste 18: Deve definir e obter o Id
        [Fact]
        public void PedidoEntity_Should_Set_And_Get_Id()
        {
            // Arrange
            var pedido = new Pedido();
            var id = 123;

            // Act
            pedido.Id = id;

            // Assert
            Assert.Equal(id, pedido.Id);
        }

        // Teste 19: Deve definir e obter o ClienteId
        [Fact]
        public void PedidoEntity_Should_Set_And_Get_ClienteId()
        {
            // Arrange
            var pedido = new Pedido();
            var id = 456;

            // Act
            // Corrigido: Propriedade é 'ClienteId'
            pedido.ClienteId = id;

            // Assert
            Assert.Equal(id, pedido.ClienteId);
        }

        // Teste 20: Deve definir e obter a Operacao (Enum)
        [Fact]
        public void PedidoEntity_Should_Set_And_Get_Operacao()
        {
            // Arrange
            var pedido = new Pedido();
            // Corrigido: Enum é 'Vendas'
            var operacao = PedidoOperacao.Vendas;

            // Act
            pedido.Operacao = operacao;

            // Assert
            Assert.Equal(operacao, pedido.Operacao);
        }

        // Teste 21: Deve definir e obter o Status (Enum)
        [Fact]
        public void PedidoEntity_Should_Set_And_Get_Status()
        {
            // Arrange
            var pedido = new Pedido();
            // Corrigido: Enum é 'Finalizado', não 'Concluido'
            var status = PedidoStatus.Finalizado;

            // Act
            pedido.Status = status;

            // Assert
            Assert.Equal(status, pedido.Status);
        }

        // Teste 22: Deve definir e obter a DataPedido
        [Fact]
        public void PedidoEntity_Should_Set_And_Get_DataPedido()
        {
            // Arrange
            var pedido = new Pedido();
            var data = DateTime.Now;

            // Act
            // Corrigido: Propriedade é 'DataPedido'
            pedido.DataPedido = data;

            // Assert
            Assert.Equal(data, pedido.DataPedido);
        }

        // Teste 23: Deve definir e obter a DataEntrega (Teste corrigido)
        [Fact]
        public void PedidoEntity_Should_Set_And_Get_DataEntrega()
        {
            // Arrange
            var pedido = new Pedido();
            var data = DateTime.Now.AddDays(1);

            // Act
            // Corrigido: Testando 'DataEntrega' em vez de 'DataAtualizacao' (que não existe)
            pedido.DataEntrega = data;

            // Assert
            Assert.Equal(data, pedido.DataEntrega);
        }

        // Teste 24: Deve definir e obter o ValorTotal
        [Fact]
        public void PedidoEntity_Should_Set_And_Get_ValorTotal()
        {
            // Arrange
            var pedido = new Pedido();
            var total = 150.75m;

            // Act
            // Corrigido: Propriedade é 'ValorTotal'
            pedido.ValorTotal = total;

            // Assert
            Assert.Equal(total, pedido.ValorTotal);
        }

        // Teste 25: Deve definir e obter a FormaPagamento
        [Fact]
        public void PedidoEntity_Should_Set_And_Get_FormaPagamento()
        {
            // Arrange
            var pedido = new Pedido();
            var formaId = (int)FormasPagamento.Pix;

            // Act
            // Corrigido: Propriedade é 'FormaPagamento'
            pedido.FormaPagamento = formaId;

            // Assert
            Assert.Equal(formaId, pedido.FormaPagamento);
        }

        // Teste 26: Deve definir e obter as Observacoes
        [Fact]
        public void PedidoEntity_Should_Set_And_Get_Observacoes()
        {
            // Arrange
            var pedido = new Pedido();
            var obs = "Entregar com urgência";

            // Act
            pedido.Observacoes = obs;

            // Assert
            Assert.Equal(obs, pedido.Observacoes);
        }

        // Teste 27: Observacoes deve aceitar null
        [Fact]
        public void PedidoEntity_Should_Allow_Null_Observacoes()
        {
            // Arrange
            var pedido = new Pedido();

            // Act
            pedido.Observacoes = null;

            // Assert
            Assert.Null(pedido.Observacoes);
        }

        // --- Testes de Valores Padrão ---

        // Teste 28: Pedido recém-criado deve ter Id 0
        [Fact]
        public void PedidoEntity_Should_Have_Default_Id_Zero()
        {
            // Arrange
            var pedido = new Pedido();

            // Assert
            Assert.Equal(0, pedido.Id);
        }

        // Teste 29: Pedido recém-criado deve ter ClienteId null
        [Fact]
        public void PedidoEntity_Should_Have_Default_ClienteId_Null()
        {
            // Arrange
            var pedido = new Pedido();

            // Assert
            // Corrigido: 'ClienteId' é 'int?', o padrão é null
            Assert.Null(pedido.ClienteId);
        }

        // Teste 30: Pedido recém-criado deve ter Status padrão (null)
        [Fact]
        public void PedidoEntity_Should_Have_Default_Status_Null()
        {
            // Arrange
            var pedido = new Pedido();

            // Assert
            // Corrigido: 'Status' é 'PedidoStatus?', o padrão é null
            Assert.Null(pedido.Status);
        }

        // Teste 31: Pedido recém-criado deve ter Operacao padrão (null)
        [Fact]
        public void PedidoEntity_Should_Have_Default_Operacao_Null()
        {
            // Arrange
            var pedido = new Pedido();

            // Assert
            // Corrigido: 'Operacao' é 'PedidoOperacao?', o padrão é null
            Assert.Null(pedido.Operacao);
        }

        // Teste 32: Pedido recém-criado deve ter ValorTotal null
        [Fact]
        public void PedidoEntity_Should_Have_Default_ValorTotal_Null()
        {
            // Arrange
            var pedido = new Pedido();

            // Assert
            // Corrigido: Propriedade é 'ValorTotal' e é 'decimal?', o padrão é null
            Assert.Null(pedido.ValorTotal);
        }
    }

    // #################################################################
    // Testes da Entidade: PedidoItem (8 Testes)
    // #################################################################
    public class PedidoItemTests
    {
        // Teste 33: Deve definir e obter o Id
        [Fact]
        public void PedidoItemEntity_Should_Set_And_Get_Id()
        {
            // Arrange
            var item = new PedidoItem();
            var id = 1;

            // Act
            item.Id = id;

            // Assert
            Assert.Equal(id, item.Id);
        }

        // Teste 34: Deve definir e obter o id_Pedido
        [Fact]
        public void PedidoItemEntity_Should_Set_And_Get_id_Pedido()
        {
            // Arrange
            var item = new PedidoItem();
            var id = 10;

            // Act
            // Corrigido: Propriedade é 'id_Pedido'
            item.id_Pedido = id;

            // Assert
            Assert.Equal(id, item.id_Pedido);
        }

        // Teste 35: Deve definir e obter o ProdutoId
        [Fact]
        public void PedidoItemEntity_Should_Set_And_Get_ProdutoId()
        {
            // Arrange
            var item = new PedidoItem();
            var id = 100;

            // Act
            // Corrigido: Propriedade é 'ProdutoId'
            item.ProdutoId = id;

            // Assert
            Assert.Equal(id, item.ProdutoId);
        }

        // Teste 36: Deve definir e obter a Quantidade
        [Fact]
        public void PedidoItemEntity_Should_Set_And_Get_Quantidade()
        {
            // Arrange
            var item = new PedidoItem();
            var qtd = 5;

            // Act
            item.Quantidade = qtd;

            // Assert
            Assert.Equal(qtd, item.Quantidade);
        }

        // Teste 37: Deve definir e obter o ValorTotal
        [Fact]
        public void PedidoItemEntity_Should_Set_And_Get_ValorTotal()
        {
            // Arrange
            var item = new PedidoItem();
            var valor = 10.50m;

            // Act
            // Corrigido: Propriedade é 'ValorTotal'
            item.ValorTotal = valor;

            // Assert
            Assert.Equal(valor, item.ValorTotal);
        }

        // --- Testes de Valores Padrão ---

        // Teste 38: Item recém-criado deve ter Id 0
        [Fact]
        public void PedidoItemEntity_Should_Have_Default_Id_Zero()
        {
            // Arrange
            var item = new PedidoItem();

            // Assert
            Assert.Equal(0, item.Id);
        }

        // Teste 39: Item recém-criado deve ter Quantidade 0
        [Fact]
        public void PedidoItemEntity_Should_Have_Default_Quantidade_Zero()
        {
            // Arrange
            var item = new PedidoItem();

            // Assert
            Assert.Equal(0, item.Quantidade);
        }

        // Teste 40: Item recém-criado deve ter ValorTotal 0
        [Fact]
        public void PedidoItemEntity_Should_Have_Default_ValorTotal_Zero()
        {
            // Arrange
            var item = new PedidoItem();

            // Assert
            // Corrigido: Propriedade é 'ValorTotal' (padrão de decimal é 0m)
            Assert.Equal(0m, item.ValorTotal);
        }
    }

    // #################################################################
    // Testes da Entidade: Movimentacao (14 Testes)
    // #################################################################
    public class MovimentacaoTests
    {
        // Teste 41: Deve definir e obter o IdMovimentacao
        [Fact]
        public void MovimentacaoEntity_Should_Set_And_Get_IdMovimentacao()
        {
            // Arrange
            var mov = new Movimentacao();
            var id = 1;

            // Act
            // Corrigido: Propriedade é 'IdMovimentacao'
            mov.IdMovimentacao = id;

            // Assert
            Assert.Equal(id, mov.IdMovimentacao);
        }

        // Teste 42: Deve definir e obter o IdProduto
        [Fact]
        public void MovimentacaoEntity_Should_Set_And_Get_IdProduto()
        {
            // Arrange
            var mov = new Movimentacao();
            var id = 100;

            // Act
            mov.IdProduto = id;

            // Assert
            Assert.Equal(id, mov.IdProduto);
        }

        // Teste 43: Deve definir e obter o TipoMovimentacao (Entrada)
        [Fact]
        public void MovimentacaoEntity_Should_Set_And_Get_Tipo_Entrada()
        {
            // Arrange
            var mov = new Movimentacao();
            // Corrigido: Tipo é Enum, não string
            var tipo = TipoMovimentacao.Entrada;

            // Act
            // Corrigido: Propriedade é 'TipoMovimentacao'
            mov.TipoMovimentacao = tipo;

            // Assert
            Assert.Equal(tipo, mov.TipoMovimentacao);
        }

        // Teste 44: Deve definir e obter o TipoMovimentacao (Saida)
        [Fact]
        public void MovimentacaoEntity_Should_Set_And_Get_Tipo_Saida()
        {
            // Arrange
            var mov = new Movimentacao();
            // Corrigido: Tipo é Enum, não string
            var tipo = TipoMovimentacao.Saida;

            // Act
            // Corrigido: Propriedade é 'TipoMovimentacao'
            mov.TipoMovimentacao = tipo;

            // Assert
            Assert.Equal(tipo, mov.TipoMovimentacao);
        }

        // Teste 45: Deve definir e obter a Quantidade
        [Fact]
        public void MovimentacaoEntity_Should_Set_And_Get_Quantidade()
        {
            // Arrange
            var mov = new Movimentacao();
            var qtd = 50;

            // Act
            mov.Quantidade = qtd;

            // Assert
            Assert.Equal(qtd, mov.Quantidade);
        }

        // Teste 46: Deve definir e obter a DataMovimentacao
        [Fact]
        public void MovimentacaoEntity_Should_Set_And_Get_DataMovimentacao()
        {
            // Arrange
            var mov = new Movimentacao();
            var data = DateTime.Now;

            // Act
            // Corrigido: Propriedade é 'DataMovimentacao'
            mov.DataMovimentacao = data;

            // Assert
            Assert.Equal(data, mov.DataMovimentacao);
        }

        // Teste 47: Deve definir e obter a Observacao
        [Fact]
        public void MovimentacaoEntity_Should_Set_And_Get_Observacao()
        {
            // Arrange
            var mov = new Movimentacao();
            var obs = "Nota Fiscal 123";

            // Act
            // Corrigido: Propriedade é 'Observacao'
            mov.Observacao = obs;

            // Assert
            Assert.Equal(obs, mov.Observacao);
        }

        // Teste 48: Observacao deve aceitar null
        [Fact]
        public void MovimentacaoEntity_Should_Allow_Null_Observacao()
        {
            // Arrange
            var mov = new Movimentacao();

            // Act
            // Corrigido: Propriedade é 'Observacao'
            mov.Observacao = null;

            // Assert
            Assert.Null(mov.Observacao);
        }

        // --- Testes de Valores Padrão ---

        // Teste 49: Movimentacao recém-criada deve ter IdMovimentacao 0
        [Fact]
        public void MovimentacaoEntity_Should_Have_Default_IdMovimentacao_Zero()
        {
            // Arrange
            var mov = new Movimentacao();

            // Assert
            // Corrigido: Propriedade é 'IdMovimentacao'
            Assert.Equal(0, mov.IdMovimentacao);
        }

        // Teste 50: Movimentacao recém-criada deve ter IdProduto null
        [Fact]
        public void MovimentacaoEntity_Should_Have_Default_IdProduto_Null()
        {
            // Arrange
            var mov = new Movimentacao();

            // Assert
            // Corrigido: 'IdProduto' é 'int?', padrão é null
            Assert.Null(mov.IdProduto);
        }

        // Teste 51: Movimentacao recém-criada deve ter Quantidade null
        [Fact]
        public void MovimentacaoEntity_Should_Have_Default_Quantidade_Null()
        {
            // Arrange
            var mov = new Movimentacao();

            // Assert
            // Corrigido: 'Quantidade' é 'int?', padrão é null
            Assert.Null(mov.Quantidade);
        }

        // Teste 52: Movimentacao recém-criada deve ter TipoMovimentacao Saida (default 0)
        [Fact]
        public void MovimentacaoEntity_Should_Have_Default_TipoMovimentacao_Saida()
        {
            // Arrange
            var mov = new Movimentacao();

            // Assert
            // Corrigido: 'TipoMovimentacao' é enum (não nulo), padrão é 0 (Saida)
            Assert.Equal(TipoMovimentacao.Saida, mov.TipoMovimentacao);
        }

        // Teste 53: Movimentacao recém-criada deve ter DataMovimentacao null
        [Fact]
        public void MovimentacaoEntity_Should_Have_Default_DataMovimentacao_Null()
        {
            // Arrange
            var mov = new Movimentacao();

            // Assert
            // Corrigido: 'DataMovimentacao' é 'DateTime?', padrão é null
            Assert.Null(mov.DataMovimentacao);
        }

        // Teste 54: Movimentacao recém-criada deve ter Observacao nula
        [Fact]
        public void MovimentacaoEntity_Should_Have_Default_Observacao_Null()
        {
            // Arrange
            var mov = new Movimentacao();

            // Assert
            // Corrigido: Propriedade é 'Observacao' (string?), padrão é null
            Assert.Null(mov.Observacao);
        }
    }

    // #################################################################
    // Testes da Entidade: Devolucao (7 Testes)
    // #################################################################
    public class DevolucaoTests
    {
        // Teste 55: Deve definir e obter o IdDevolucao
        [Fact]
        public void DevolucaoEntity_Should_Set_And_Get_IdDevolucao()
        {
            // Arrange
            var dev = new Devolucao();
            var id = 1;

            // Act
            // Corrigido: Propriedade é 'IdDevolucao'
            dev.IdDevolucao = id;

            // Assert
            Assert.Equal(id, dev.IdDevolucao);
        }

        // Teste 56: (Removido - IdPedido não existe em Devolucao)
        // Teste 57: (Removido - IdCliente não existe em Devolucao)

        // Teste 58: Deve definir e obter a DataDevolucao
        [Fact]
        public void DevolucaoEntity_Should_Set_And_Get_DataDevolucao()
        {
            // Arrange
            var dev = new Devolucao();
            var data = DateTime.Now;

            // Act
            dev.DataDevolucao = data;

            // Assert
            Assert.Equal(data, dev.DataDevolucao);
        }

        // Teste 59: Deve definir e obter a Observacao
        [Fact]
        public void DevolucaoEntity_Should_Set_And_Get_Observacao()
        {
            // Arrange
            var dev = new Devolucao();
            var motivo = "Produto errado";

            // Act
            // Corrigido: Propriedade é 'Observacao'
            dev.Observacao = motivo;

            // Assert
            Assert.Equal(motivo, dev.Observacao);
        }

        // Teste 60: (Removido - ValorDevolucao não existe em Devolucao)

        // --- Testes de Valores Padrão ---

        // Teste 61: Devolucao recém-criada deve ter IdDevolucao 0
        [Fact]
        public void DevolucaoEntity_Should_Have_Default_IdDevolucao_Zero()
        {
            // Arrange
            var dev = new Devolucao();

            // Assert
            // Corrigido: Propriedade é 'IdDevolucao'
            Assert.Equal(0, dev.IdDevolucao);
        }

        // Teste 62: (Removido - IdPedido não existe)

        // Teste 63: Devolucao recém-criada deve ter DataDevolucao com valor padrão
        [Fact]
        public void DevolucaoEntity_Should_Have_Default_DataDevolucao()
        {
            // Arrange
            var dev = new Devolucao();

            // Assert
            // Corrigido: 'DataDevolucao' é 'DateTime' (não nulo), padrão é MinValue
            Assert.Equal(DateTime.MinValue, dev.DataDevolucao);
        }

        // Teste 64: Devolucao recém-criada deve ter Observacao nula
        [Fact]
        public void DevolucaoEntity_Should_Have_Default_Observacao_Null()
        {
            // Arrange
            var dev = new Devolucao();

            // Assert
            // Corrigido: Propriedade é 'Observacao' (string?), padrão é null
            Assert.Null(dev.Observacao);
        }
    }

    // #################################################################
    // Testes da Entidade: DevolucaoItem (7 Testes)
    // #################################################################
    public class DevolucaoItemTests
    {
        // Teste 65: Deve definir e obter o IdDevolucaoItem
        [Fact]
        public void DevolucaoItemEntity_Should_Set_And_Get_IdDevolucaoItem()
        {
            // Arrange
            var item = new DevolucaoItem();
            var id = 1;

            // Act
            // Corrigido: Propriedade é 'IdDevolucaoItem'
            item.IdDevolucaoItem = id;

            // Assert
            Assert.Equal(id, item.IdDevolucaoItem);
        }

        // Teste 66: Deve definir e obter o IdDevolucao
        [Fact]
        public void DevolucaoItemEntity_Should_Set_And_Get_IdDevolucao()
        {
            // Arrange
            var item = new DevolucaoItem();
            var id = 10;

            // Act
            item.IdDevolucao = id;

            // Assert
            Assert.Equal(id, item.IdDevolucao);
        }

        // Teste 67: Deve definir e obter o IdProduto
        [Fact]
        public void DevolucaoItemEntity_Should_Set_And_Get_IdProduto()
        {
            // Arrange
            var item = new DevolucaoItem();
            var id = 100;

            // Act
            item.IdProduto = id;

            // Assert
            Assert.Equal(id, item.IdProduto);
        }

        // Teste 68: Deve definir e obter a QuantidadeDevolvida
        [Fact]
        public void DevolucaoItemEntity_Should_Set_And_Get_QuantidadeDevolvida()
        {
            // Arrange
            var item = new DevolucaoItem();
            var qtd = 5;

            // Act
            item.QuantidadeDevolvida = qtd;

            // Assert
            Assert.Equal(qtd, item.QuantidadeDevolvida);
        }

        // --- Testes de Valores Padrão ---

        // Teste 69: Item recém-criado deve ter IdDevolucaoItem 0
        [Fact]
        public void DevolucaoItemEntity_Should_Have_Default_IdDevolucaoItem_Zero()
        {
            // Arrange
            var item = new DevolucaoItem();

            // Assert
            // Corrigido: Propriedade é 'IdDevolucaoItem'
            Assert.Equal(0, item.IdDevolucaoItem);
        }

        // Teste 70: Item recém-criado deve ter IdDevolucao 0
        [Fact]
        public void DevolucaoItemEntity_Should_Have_Default_IdDevolucao_Zero()
        {
            // Arrange
            var item = new DevolucaoItem();

            // Assert
            Assert.Equal(0, item.IdDevolucao);
        }

        // Teste 71: Item recém-criado deve ter QuantidadeDevolvida 0
        [Fact]
        public void DevolucaoItemEntity_Should_Have_Default_QuantidadeDevolvida_Zero()
        {
            // Arrange
            var item = new DevolucaoItem();

            // Assert
            Assert.Equal(0, item.QuantidadeDevolvida);
        }
    }

    // #################################################################
    // Testes da Entidade: MasterData (10 Testes)
    // #################################################################
    public class MasterDataTests
    {
        // Teste 72: Deve definir e obter o Name
        [Fact]
        public void MasterData_Should_Set_And_Get_Name()
        {
            // Arrange
            var data = new MasterData();
            var name = "TestName";
            // Act
            data.Name = name;
            // Assert
            Assert.Equal(name, data.Name);
        }

        // Teste 73: Deve definir e obter o Type
        [Fact]
        public void MasterData_Should_Set_And_Get_Type()
        {
            // Arrange
            var data = new MasterData();
            var type = "A";
            // Act
            data.Type = type;
            // Assert
            Assert.Equal(type, data.Type);
        }

        // Teste 74: Deve definir e obter o TableName
        [Fact]
        public void MasterData_Should_Set_And_Get_TableName()
        {
            // Arrange
            var data = new MasterData();
            var table = "Clientes";
            // Act
            data.TableName = table;
            // Assert
            Assert.Equal(table, data.TableName);
        }

        // Teste 75: Deve definir e obter o Json
        [Fact]
        public void MasterData_Should_Set_And_Get_Json()
        {
            // Arrange
            var data = new MasterData();
            var json = "{\"key\":\"value\"}";
            // Act
            data.Json = json;
            // Assert
            Assert.Equal(json, data.Json);
        }

        // Teste 76: Deve definir e obter o Info
        [Fact]
        public void MasterData_Should_Set_And_Get_Info()
        {
            // Arrange
            var data = new MasterData();
            var info = "Some info";
            // Act
            data.Info = info;
            // Assert
            Assert.Equal(info, data.Info);
        }

        // Teste 77: Deve definir e obter o Owner
        [Fact]
        public void MasterData_Should_Set_And_Get_Owner()
        {
            // Arrange
            var data = new MasterData();
            var owner = "dbo";
            // Act
            data.Owner = owner;
            // Assert
            Assert.Equal(owner, data.Owner);
        }

        // Teste 78: Deve definir e obter o Modified
        [Fact]
        public void MasterData_Should_Set_And_Get_Modified()
        {
            // Arrange
            var data = new MasterData();
            var date = DateTime.Now;
            // Act
            data.Modified = date;
            // Assert
            Assert.Equal(date, data.Modified);
        }

        // Teste 79: Deve definir e obter o Sync
        [Fact]
        public void MasterData_Should_Set_And_Get_Sync()
        {
            // Arrange
            var data = new MasterData();
            // Act
            data.Sync = true;
            // Assert
            Assert.True(data.Sync);
        }

        // Teste 80: Deve ter valores padrão nulos (para anuláveis) e MinValue (para DateTime)
        [Fact]
        public void MasterData_Should_Have_Default_Values()
        {
            // Arrange
            var data = new MasterData();
            // Act
            // Assert
            Assert.Null(data.Name); // string = null!
            Assert.Null(data.Type); // string = null!
            Assert.Null(data.TableName);
            Assert.Null(data.Json);
            Assert.Null(data.Info);
            Assert.Null(data.Owner);
            Assert.Equal(DateTime.MinValue, data.Modified); // DateTime não anulável
            Assert.Null(data.Sync); // bool?
        }

        // Teste 81: Propriedades anuláveis devem aceitar null
        [Fact]
        public void MasterData_Should_Allow_Nulls_For_Nullable_Properties()
        {
            // Arrange
            var data = new MasterData
            {
                TableName = "Test",
                Json = "Test",
                Info = "Test",
                Owner = "Test",
                Sync = false
            };

            // Act
            data.TableName = null;
            data.Json = null;
            data.Info = null;
            data.Owner = null;
            data.Sync = null;

            // Assert
            Assert.Null(data.TableName);
            Assert.Null(data.Json);
            Assert.Null(data.Info);
            Assert.Null(data.Owner);
            Assert.Null(data.Sync);
        }
    }
}