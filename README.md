# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">

Plano de Testes de Unidade: Validação da Entidade Cliente

Versão: 1.0
Data: 04 de Novembro de 2025
Autor: Gean Carlos de Sousa Bandeira

1. Introdução e Objetivo

Este Pull Request estabelece a cobertura de testes unitários para a entidade Cliente (Cliente.cs), que é um modelo de dados central no domínio da aplicação.

O objetivo é assegurar que a entidade Cliente se comporta como esperado, validando rigorosamente seus valores padrão (default), a funcionalidade de getters e setters para todas as propriedades, e o tratamento correto de valores nulos, garantindo a integridade dos dados na camada de domínio.

2. Estratégia e Escopo

Escopo: A validação se concentra exclusivamente na classe Estoque.Domain.Entities.Cliente.

Estratégia: Segue o padrão "Arrange, Act, Assert". Os testes são puramente unitários e não utilizam mocks ou simulações, focando apenas no estado do objeto da entidade e suas propriedades.

Ferramentas: xUnit.

3. Casos de Teste Adicionados (Total: 20 Testes)

A suíte ClienteTests foi criada (ou atualizada) para cobrir os seguintes cenários:

3.1. Testes de Valores Padrão (Default)

Garantem que um new Cliente() é instanciado em um estado conhecido e seguro.

ClienteEntity_Should_Have_Default_Id_Zero: Verifica se o Id inicial é 0.

ClienteEntity_Should_Have_Default_Empty_Nome: Verifica se Nome inicial é string.Empty (e não null).

ClienteEntity_Should_Have_Default_Status: Verifica se o Status inicial é UserStatus.Inativo (valor 0 do enum).

ClienteEntity_Should_Have_Default_Tipo: Verifica se o Tipo inicial é (TipoCliente)0 (valor 0 do enum).

ClienteEntity_Should_Have_Default_DataCadastro_Min: Verifica se DataCadastro inicial é DateTime.MinValue.

3.2. Testes de Propriedades (Set/Get)

Validam a capacidade de ler e escrever valores em todas as propriedades da entidade.

ClienteEntity_Should_Set_And_Get_Id

ClienteEntity_Should_Set_And_Get_Nome

ClienteEntity_Should_Set_And_Get_Documento

ClienteEntity_Should_Set_And_Get_Email

ClienteEntity_Should_Set_And_Get_NomeFantasia

ClienteEntity_Should_Set_And_Get_NomeContato

ClienteEntity_Should_Set_And_Get_Telefone

ClienteEntity_Should_Set_And_Get_Anexo

ClienteEntity_Should_Set_DataCadastro

ClienteEntity_Should_Allow_Setting_Nome_To_Empty

3.3. Testes de Enum (Set/Get)

Validam a atribuição de valores específicos dos Enums.

ClienteEntity_Should_Set_And_Get_Status (Testa com UserStatus.Ativo)

ClienteEntity_Should_Set_And_Get_Status_Inativo

ClienteEntity_Should_Set_And_Get_Tipo (Testa com TipoCliente.Fisica)

ClienteEntity_Should_Set_And_Get_Tipo_Juridica

3.4. Testes de Nulabilidade

Garante que as propriedades anuláveis (marcadas com ?) aceitam null corretamente.

ClienteEntity_Should_Allow_Null_Optional_Properties: Testa Documento, NomeContato, Telefone, Anexo, NomeFantasia e Email recebendo null.

4. Como Executar os Testes

Navegue até a pasta raiz da solução (src/) pelo terminal.

Para rodar todos os testes do projeto:

dotnet test


Para rodar APENAS esta suíte de testes (ClienteTests):

dotnet test --filter "FullyQualifiedName~Estoque.Tests.Services.ClienteTests"


Verifique se todos os 20 testes da suíte Estoque.Tests.Services.ClienteTests passam com sucesso.
