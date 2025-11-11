# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">

# Plano de Testes de Unidade: Validação da Entidade Cliente

**Versão:** 1.0  
**Data:** 04 de Novembro de 2025  
**Autor:** Gean Carlos de Sousa Bandeira

## 1. Introdução e Objetivo

Este Pull Request introduz a cobertura de testes de unidade para a entidade `Cliente` (`Estoque.Domain.Entities.Cliente`).

O objetivo é assegurar que a entidade se comporte como esperado em sua instanciação (valores padrão) e que todas as suas propriedades (`get` e `set`) funcionem corretamente, incluindo o tratamento de valores opcionais (nulos). Isso garante a integridade e a previsibilidade do estado da entidade antes de ser usada em serviços ou controllers.

## 2. Estratégia e Escopo

* **Escopo:** A validação se concentra exclusivamente na entidade `Cliente`.
* **Estratégia:** Segue o padrão "Arrange, Act, Assert". Os testes instanciam a entidade e validam seu estado diretamente. Não há mocks, pois não há dependências externas.
* **Ferramentas:** xUnit.

## 3. Casos de Teste Detalhados Adicionados

| ID | Classe/Componente | Descrição do Teste | Cenário | Resultado Esperado | Cobertura da Lógica |
| :--- | :--- | :--- | :--- | :--- | :--- |
| TU-CLI-001 | `ClienteTests` | Garantir valores padrão na instanciação da entidade. | 1. Uma nova instância de `Cliente` é criada (`new Cliente()`). | A entidade deve ser criada com `Id = 0`, `Nome = string.Empty`, `Status = UserStatus.Inativo` (0), `Tipo = (TipoCliente)0`, e `DataCadastro = DateTime.MinValue`. | Valida o estado inicial padrão da entidade. (Cobre os testes 8, 9, 10, 11, 12) |
| TU-CLI-002 | `ClienteTests` | Validar atribuição e obtenção (Set/Get) das propriedades. | 1. Valores válidos são atribuídos a todas as propriedades (Id, Nome, Documento, Email, Status, Tipo, Contato, Telefone, etc.). | As propriedades devem reter e retornar os exatos valores que foram atribuídos a elas. | Cobre o "caminho feliz" para a mutação de estado da entidade. (Cobre os testes 1, 2, 3, 4, 5, 7, 13, 14, 15, 16, 17, 18, 19, 20) |
| TU-CLI-003 | `ClienteTests` | Garantir que propriedades opcionais (anuláveis) aceitem `null`. | 1. O valor `null` é atribuído a campos como `Documento`, `NomeContato`, `Telefone`, `Anexo`, `NomeFantasia` e `Email`. | A entidade deve aceitar `null` nessas propriedades sem lançar exceções. | Assegura que a entidade respeita a nulidade de campos não obrigatórios. (Cobre o teste 6) |

## 4. Como Executar os Testes

1.  Navegue até a pasta raiz da solução (ou do projeto de testes) pelo terminal.
2.  Execute o comando:
    ```bash
    dotnet test
    ```
3.  Verifique se todos os **20 testes** da suíte `Estoque.Tests.Services.ClienteTests` passam com sucesso.
