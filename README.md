# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">


# Plano de Testes de Unidade: Segurança, Autenticação e Identity (Fase 3)

**Versão:** 1.2 (Finalização dos Unitários)
**Data:** 22 de Novembro de 2025
**Autor:** Gean Carlos de Sousa Bandeira

## 1. Introdução e Objetivo

Este Pull Request conclui o ciclo de testes unitários do projeto **Estoque**, focando na camada mais crítica e sensível do sistema: **Segurança e Controle de Acesso**.

Foram implementados testes para validar o fluxo de autenticação (`AuthService`), a verificação de permissões de acesso (`RoleService`) e a localização das mensagens de erro do Identity (`IdentityErrorDescriber`).

## 2. Desafios Técnicos Superados

Para testar o `AuthService`, foi necessário implementar uma estratégia avançada de **Mocking** para simular o comportamento do ASP.NET Identity, que é altamente acoplado ao contexto HTTP.

* **Mock do `SignInManager`:** Simulação de sucesso e falha no login sem necessidade de banco de dados real.
* **Mock do `HttpContext` e `IServiceProvider`:** Injeção manual de um `IAuthenticationService` falso para permitir que o método `SignInAsync` funcionasse em ambiente de teste isolado.

## 3. Casos de Teste Implementados

| Componente | Método Testado | Cenário | Resultado Esperado |
| :--- | :--- | :--- | :--- |
| **AuthService** | `SignInAsync` | Login com credenciais válidas. | Retornar `Success = true` e nenhuma mensagem de erro. |
| **AuthService** | `SignInAsync` | Login com usuário inexistente. | Retornar `Success = false` e mensagem de erro apropriada. |
| **RoleService** | `HasAccessAsync` | Verificação de acesso a menu (Role vinculada). | Retornar `true` se existir registro na tabela `RoleMenus`. |
| **RoleService** | `HasAccessAsync` | Verificação de acesso negado. | Retornar `false` se não houver vínculo no banco. |
| **IdentityErrorDescriber** | `PasswordTooShort` | Validação de senha curta. | Retornar mensagem traduzida: "A senha deve ter no mínimo 6 caracteres." |
| **IdentityErrorDescriber** | `DefaultError` | Erro genérico. | Retornar mensagem padrão da base (inglês) ou traduzida (se implementada). |

## 4. Cobertura Total (Fases 1, 2 e 3)

Com a aprovação deste PR, o projeto alcança uma cobertura unitária robusta nas seguintes áreas:

* ✅ **Domain:** Entidades, Enums e Models.
* ✅ **Services:** Pedidos, Movimentações (Estoque), Usuários, Clientes, Fornecedores, Categorias, Auth e Roles.
* ✅ **Utils:** Cálculos Fiscais (Danfe), Helpers e Extensions.

---
**Status:** ✅ Todos os testes executados com sucesso.
