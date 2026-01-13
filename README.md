# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">

## 📌 Sobre o Projeto
Sistema completo de gestão de estoque desenvolvido para a **VIP Penha**, loja especializada em eletrônicos. Oferece controle de produtos, movimentações, fornecedores e relatórios integrados.

# Plano de Testes de Unidade: Serviços de Gestão e Controle de Estoque (Fase 2)

**Versão:** 1.1  
**Data:** 22 de Novembro de 2025  
**Autor:** Gean Carlos de Sousa Bandeira

## 1. Introdução e Objetivo

Dando continuidade à estratégia de qualidade de software, este Pull Request (Fase 2) foca na cobertura de testes unitários para os Serviços de Cadastro (`Categoria`, `Fornecedor`), Gestão de Usuários (`UserService`) e, crucialmente, a Lógica de Movimentação de Estoque (`MovimentacaoService`).

O objetivo é blindar as regras de negócio que afetam diretamente a integridade dos dados e o saldo de produtos no estoque.

## 2. Estratégia e Decisões Técnicas

* **Escopo:** Camada `Infrastructure.Services` e validação de `Domain.Models`.
* **Isolamento de UI:** Para o `UserService`, optou-se por testar exclusivamente a **lógica de negócio** (contagem de ativos, recuperação de avatar), isolando a complexidade de componentes visuais (`JJFormView`) que demandariam testes de integração mais pesados.
* **Controle de Estoque:** Implementamos testes rigorosos para o `MovimentacaoService`, simulando cenários de entrada (criação de lote) e saída (baixa de estoque), garantindo que o sistema impeça vendas sem saldo.

## 3. Casos de Teste Implementados

Abaixo, a matriz de cobertura adicionada neste PR:

| Componente / Serviço | Cenário de Teste | Resultado Esperado |
| :--- | :--- | :--- |
| **MovimentacaoService** | Registrar Entrada | Deve criar um novo `ProdutoLote` com a quantidade correta e saldo disponível. |
| **MovimentacaoService** | Registrar Saída (Saldo Suficiente) | Deve decrementar a quantidade disponível no lote mais antigo (FIFO/LIFO conforme lógica). |
| **MovimentacaoService** | Registrar Saída (Saldo Insuficiente) | **Deve lançar `InvalidOperationException`**, impedindo a operação negativa. |
| **UserService** | Contagem de Ativos | O método `GetActiveUsersCount` deve filtrar corretamente apenas usuários com status `Ativo`. |
| **UserService** | Avatar do Usuário | O método `GetUserAvatarBytes` deve retornar `null` de forma segura se o usuário não possuir foto. |
| **CategoriaService** | Criação de View | O método `GetFormViewCategoriaAsync` deve retornar o objeto `JJFormView` configurado corretamente via Mock. |
| **FornecedorService** | Criação de View | O método `GetFormViewFornecedorAsync` deve instanciar a View de gerenciamento de fornecedores. |
| **Model Validation** | ViewModels (EditUser, ResetPassword) | Validação dos atributos `[Required]` e `[Compare]` (confirmação de senha). |

## 4. Ferramentas Utilizadas

* **xUnit:** Framework de testes.
* **Moq:** Para simular dependências como `IWebHostEnvironment`, `IComponentFactory` e `SignInManager`.
* **EF Core InMemory:** Para validar a persistência real das movimentações de estoque e consultas de usuários em um banco volátil.
* **FormatterServices:** Utilizado para instanciar objetos complexos de UI sem dependências de construtor quando necessário.

---
**Status:** ✅ Testes executados e aprovados.
