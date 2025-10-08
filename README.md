# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">

## 📌 Sobre o Projeto
Sistema completo de gestão de estoque desenvolvido para a **VIP Penha**, loja especializada em eletrônicos. Oferece controle de produtos, movimentações, fornecedores e relatórios integrados.

# Plano de Testes de Unidade: Validação do UserController

**Versão:** 1.0
**Data:** 08 de Outubro de 2025
**Autor:** Gean Carlos de Sousa Bandeira

### 1. Introdução e Objetivo

Este documento descreve o plano e os casos de teste de unidade criados para validar as funcionalidades do `UserController`, que é responsável pelo gerenciamento de usuários no sistema.

O objetivo principal é garantir que as actions do controller, como a criação e visualização de detalhes de usuários, se comportem de maneira previsível e segura, tratando corretamente tanto os cenários de sucesso quanto os de erro, de forma totalmente isolada de dependências externas (como o banco de dados).

### 2. Estratégia e Escopo

* **Escopo:** A validação se concentra nas actions `UserDetails` (GET) e `CreateUser` (POST) do `UserController`.
* **Tipo de Teste:** Teste de Unidade (automatizado), utilizando um banco de dados em memória e mocks para simular dependências.
* **Estratégia:** Foi utilizado o padrão "Arrange, Act, Assert" (Preparar, Agir, Verificar). As dependências externas, como o `UserManager`, foram "mockadas" para simular diferentes respostas e garantir o isolamento dos testes.
* **Ferramentas:**
    * **Framework de Teste:** xUnit
    * **Simulação (Mocks):** Moq
    * **Asserções (Verificações):** FluentAssertions

### 3. Casos de Teste Detalhados

| ID | Controller | Descrição do Teste | Cenário | Resultado Esperado | Cobertura da Lógica |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **TU-USR-001** | `UserController` | **`UserDetails` (GET):** Garantir que um erro 404 (Não Encontrado) é retornado se o usuário não existe. | 1. O `UserManager` é configurado para retornar `null` ao buscar um usuário por ID. <br> 2. A action `UserDetails` é chamada com um ID qualquer. | O resultado da action deve ser do tipo `NotFoundResult`. | Valida o tratamento de erro para consulta de usuários inexistentes. |
| **TU-USR-002** | `UserController` | **`UserDetails` (GET):** Garantir que a View correta é retornada se o usuário existe. | 1. O `UserManager` é configurado para retornar um objeto de usuário válido. <br> 2. A action `UserDetails` é chamada com o ID desse usuário. | O resultado deve ser uma `ViewResult` contendo um `Model` do tipo `EditUserViewModel`. | Cobre o "caminho feliz" da funcionalidade de visualização de detalhes do usuário. |
| **TU-USR-003** | `UserController` | **`CreateUser` (POST):** Impedir a criação de usuário com dados inválidos. | 1. Um erro de validação é adicionado manualmente ao `ModelState` do controller. <br> 2. A action `CreateUser` é chamada com um modelo inválido. | O resultado deve ser uma `ViewResult`, retornando para a mesma tela de criação com as mensagens de erro no `ModelState`. | Garante que a validação do `ViewModel` está funcionando antes de prosseguir com a lógica de negócio. |
| **TU-USR-004** | `UserController` | **`CreateUser` (POST):** Tratar falha durante a criação do usuário no Identity. | 1. O `UserManager.CreateAsync` é configurado para retornar `IdentityResult.Failed`. <br> 2. A action `CreateUser` é chamada com dados válidos. | O resultado deve ser uma `ViewResult`, e o `ModelState` deve conter os erros retornados pelo `UserManager`. | Valida o tratamento de erros provenientes da camada de identidade, exibindo-os para o usuário. |
| **TU-USR-005** | `UserController` | **`CreateUser` (POST):** Tratar falha ao adicionar o usuário a uma "Role". | 1. `CreateAsync` retorna sucesso. <br> 2. `AddToRoleAsync` é configurado para retornar `IdentityResult.Failed`. | O resultado deve ser uma `ViewResult`, e o `ModelState` deve conter os erros da atribuição da "Role". | Garante que a transação de criação de usuário é robusta, tratando falhas em etapas secundárias. |
| **TU-USR-006** | `UserController` | **`CreateUser` (POST):** Garantir o redirecionamento em caso de sucesso. | 1. `CreateAsync` e `AddToRoleAsync` são configurados para retornar `IdentityResult.Success`. <br> 2. A action é chamada com um modelo válido. | O resultado deve ser um `RedirectToActionResult` para a action `Index`, e uma mensagem de sucesso deve ser adicionada ao `TempData`. | Cobre o "caminho feliz" completo, garantindo que o usuário é redirecionado corretamente após a criação bem-sucedida. |

### 4. Como Executar os Testes

1.  Navegue até a pasta raiz da solução pelo terminal.
2.  Execute o comando:
    ```bash
    dotnet test
    ```
3.  Todos os testes de unidade descritos acima (e os demais do projeto) devem ser executados e passar com sucesso.
