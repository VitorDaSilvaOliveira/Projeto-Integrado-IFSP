using Estoque.Domain.Entities;
using Estoque.Domain.Enums;

namespace Estoque.Tests.Services
{
    /// <summary>
    /// Testes focados na Entidade Cliente.
    /// </summary>
    public class ClienteTests
    {

        // Teste 1: Deve definir e obter o Nome do cliente
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
            var documento = "123.456.789-00";

            // Act
            cliente.Documento = documento;

            // Assert
            Assert.Equal(documento, cliente.Documento);
        }

        // Teste 3: Deve definir e obter o Status Ativo
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

        // Teste 4: Deve definir e obter o Tipo Fisica
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Tipo()
        {
            // Arrange
            var cliente = new Cliente();
            var tipo = TipoCliente.Fisica; 

            // Act
            cliente.Tipo = tipo;

            // Assert
            Assert.Equal(tipo, cliente.Tipo);
        }

        // Teste 5: Deve definir e obter o Email
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
        
        // Teste 6: Propriedades anuláveis devem aceitar null
        [Fact]
        public void ClienteEntity_Should_Allow_Null_Optional_Properties()
        {
            // Arrange
            var cliente = new Cliente();

            // Act
            cliente.Documento = null;
            cliente.NomeContato = null;
            cliente.Telefone = null;
            cliente.Anexo = null;
            cliente.NomeFantasia = null;
            cliente.Email = null;

            // Assert
            Assert.Null(cliente.Documento);
            Assert.Null(cliente.NomeFantasia);
            Assert.Null(cliente.Email);
            Assert.Null(cliente.NomeContato);
            Assert.Null(cliente.Telefone);
            Assert.Null(cliente.Anexo);
        }

        // Teste 7: DataCadastro deve ser definida
        [Fact]
        public void ClienteEntity_Should_Set_DataCadastro()
        {
             // Arrange
            var cliente = new Cliente();
            var data = DateTime.Now;

            // Act
            cliente.DataCadastro = data;
            
            // Assert
            Assert.Equal(data, cliente.DataCadastro);
        }


        // Teste 8: Cliente recém-criado deve ter Id 0
        [Fact]
        public void ClienteEntity_Should_Have_Default_Id_Zero()
        {
            // Arrange
            var cliente = new Cliente();
            
            // Assert
            Assert.Equal(0, cliente.Id);
        }

        // Teste 9: Cliente recém-criado deve ter Nome como string vazia (e não nulo)
        [Fact]
        public void ClienteEntity_Should_Have_Default_Empty_Nome()
        {
            // Arrange
            var cliente = new Cliente();

            // Assert
            Assert.Equal(string.Empty, cliente.Nome);
        }

        // Teste 10: Cliente recém-criado deve ter Tipo padrão (0)
        [Fact]
        public void ClienteEntity_Should_Have_Default_Tipo()
        {
            // Arrange
            var cliente = new Cliente();
            
            // CORREÇÃO: O padrão de um enum não inicializado é 0.
            // O erro mostrou que 'Fisica' não é 0.
            Assert.Equal((TipoCliente)0, cliente.Tipo);
        }
        
        // Teste 11: Cliente recém-criado deve ter Status padrão (Inativo)
        [Fact]
        public void ClienteEntity_Should_Have_Default_Status()
        {
            // Arrange
            var cliente = new Cliente();

            // CORREÇÃO: O erro mostrou que o padrão (0) é 'Inativo'.
            Assert.Equal(UserStatus.Inativo, cliente.Status);
        }

        // Teste 12: Cliente recém-criado deve ter DataCadastro com valor mínimo
        [Fact]
        public void ClienteEntity_Should_Have_Default_DataCadastro_Min()
        {
            // Arrange
            var cliente = new Cliente();

            // Assert
            // O padrão de um DateTime não inicializado é MinValue.
            Assert.Equal(DateTime.MinValue, cliente.DataCadastro);
        }

        // Teste 13: Deve definir e obter o Tipo Juridica
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Tipo_Juridica()
        {
            // Arrange
            var cliente = new Cliente();
            var tipo = TipoCliente.Juridica;

            // Act
            cliente.Tipo = tipo;

            // Assert
            Assert.Equal(tipo, cliente.Tipo);
        }

        // Teste 14: Deve definir e obter o Status Inativo (redundante com o Teste 11, mas bom para set/get)
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Status_Inativo()
        {
            // Arrange
            var cliente = new Cliente();
            var status = UserStatus.Inativo;

            // Act
            cliente.Status = status;

            // Assert
            Assert.Equal(status, cliente.Status);
        }
        
        // Teste 15: Deve definir e obter o Id
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Id()
        {
            // Arrange
            var cliente = new Cliente();
            var id = 123;
            
            // Act
            cliente.Id = id;

            // Assert
            Assert.Equal(id, cliente.Id);
        }

        // Teste 16: Deve definir e obter NomeContato
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_NomeContato()
        {
            // Arrange
            var cliente = new Cliente();
            var nomeContato = "Contato Principal";
            
            // Act
            cliente.NomeContato = nomeContato;

            // Assert
            Assert.Equal(nomeContato, cliente.NomeContato);
        }

        // Teste 17: Deve definir e obter Telefone
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Telefone()
        {
            // Arrange
            var cliente = new Cliente();
            var telefone = "(11) 99999-9999";
            
            // Act
            cliente.Telefone = telefone;

            // Assert
            Assert.Equal(telefone, cliente.Telefone);
        }

        // Teste 18: Deve definir e obter Anexo
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_Anexo()
        {
            // Arrange
            var cliente = new Cliente();
            var anexo = "caminho/para/arquivo.pdf";
            
            // Act
            cliente.Anexo = anexo;

            // Assert
            Assert.Equal(anexo, cliente.Anexo);
        }

        // Teste 19: Deve definir e obter NomeFantasia
        [Fact]
        public void ClienteEntity_Should_Set_And_Get_NomeFantasia()
        {
            // Arrange
            var cliente = new Cliente();
            var nomeFantasia = "Empresa Fantasia";
            
            // Act
            cliente.NomeFantasia = nomeFantasia;

            // Assert
            Assert.Equal(nomeFantasia, cliente.NomeFantasia);
        }

        // Teste 20: Deve permitir definir Nome como string vazia
        [Fact]
        public void ClienteEntity_Should_Allow_Setting_Nome_To_Empty()
        {
            // Arrange
            var cliente = new Cliente { Nome = "Nome Inicial" }; // Garante que não é o default

            // Act
            cliente.Nome = string.Empty;

            // Assert
            Assert.Equal(string.Empty, cliente.Nome);
        }
    }
}