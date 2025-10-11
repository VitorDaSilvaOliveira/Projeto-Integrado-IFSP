# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">

## 📌 Sobre o Projeto
Sistema completo de gestão de estoque desenvolvido para a **VIP Penha**, loja especializada em eletrônicos. Oferece controle de produtos, movimentações, fornecedores e relatórios integrados.

# Plano de Testes de Unidade: Validação da Edição de Usuário (`UserDetails` POST)

**Versão:** 1.0
**Data:** 11 de Outubro de 2025
**Autor:** Gean Carlos de Sousa Bandeira

### 1. Introdução e Objetivo

Este Pull Request expande a cobertura de testes do `UserController`, focando especificamente na action `UserDetails` (método POST), que é responsável por processar a atualização das informações de um usuário.

O objetivo é assegurar que a lógica de edição de usuário seja robusta, tratando corretamente dados inválidos, cenários de erro e o fluxo de sucesso, garantindo a integridade dos dados e uma boa experiência para o administrador.

### 2. Estratégia e Escopo

* **Escopo:** A validação se concentra exclusivamente na action `UserDetails(EditUserViewModel model)` do `UserController`.
* **Estratégia:** Segue o padrão "Arrange, Act, Assert". As dependências (`UserManager`) são simuladas (*mockadas*) para controlar as respostas do Identity Framework e isolar o teste à lógica do controller.
* **Ferramentas:** xUnit, Moq, FluentAssertions.

### 3. Casos de Teste Detalhados Adicionados

| ID | Controller | Descrição do Teste | Cenário | Resultado Esperado | Cobertura da Lógica |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **TU-USR-007** | `UserController` | **`UserDetails` (POST):** Impedir a atualização com dados inválidos. | 1. Um erro de validação é adicionado ao `ModelState`. <br> 2. A action `UserDetails` é chamada com um `EditUserViewModel` inválido. | O resultado deve ser uma `ViewResult`, retornando à mesma página de edição com os erros de validação. | Garante que a validação do modelo é respeitada antes de tentar atualizar o usuário. |
| **TU-USR-008** | `UserController` | **`UserDetails` (POST):** Retornar 404 ao tentar atualizar um usuário que não existe. | 1. O `UserManager` é configurado para retornar `null` ao buscar pelo ID do usuário no modelo. <br> 2. A action é chamada. | O resultado deve ser um `NotFoundResult`. | Cobre o cenário de segurança em que um ID de usuário inválido ou inexistente é submetido. |
| **TU-USR-009** | `UserController` | **`UserDetails` (POST):** Tratar falha durante a atualização no Identity. | 1. `FindByIdAsync` retorna um usuário válido. <br> 2. `UpdateAsync` é configurado para retornar `IdentityResult.Failed`. | O resultado deve ser uma `ViewResult`, e o `ModelState` deve conter os erros retornados pelo `UserManager`. | Valida o tratamento de erros da camada de persistência, informando ao usuário sobre a falha. |
| **TU-USR-010** | `UserController` | **`UserDetails` (POST):** Garantir o redirecionamento em caso de sucesso na atualização. | 1. `FindByIdAsync` e `UpdateAsync` são configurados para retornar sucesso. <br> 2. A action é chamada com um modelo válido. | O resultado deve ser um `RedirectToActionResult` para a própria action `UserDetails` (GET), e uma mensagem de sucesso deve estar no `TempData`. | Cobre o "caminho feliz" completo, confirmando que a página é recarregada com os novos dados após a atualização. |

### 4. Como Executar os Testes

1.  Navegue até a pasta raiz da solução pelo terminal.
2.  Execute o comando:
    ```bash
    dotnet test
    ```
3.  Verifique se todos os 10 testes (6 anteriores e os 4 novos) da suíte `UserControllerTests` passam com sucesso.
