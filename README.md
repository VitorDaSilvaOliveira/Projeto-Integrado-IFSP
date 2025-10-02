# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">

## 📌 Sobre o Projeto
Sistema completo de gestão de estoque desenvolvido para a **VIP Penha**, loja especializada em eletrônicos. Oferece controle de produtos, movimentações, fornecedores e relatórios integrados.

# Plano de Testes de Unidade: Regras de Negócio de Domínio

**Versão:** 1.0
**Data:** 02 de Outubro de 2025
**Autor:** Gean Carlos de Sousa Bandeira

---

## 1. Introdução e Objetivo

Este documento descreve o plano e os casos de teste de unidade criados para validar regras de negócio críticas contidas nas entidades de domínio do sistema VIP PENHA, especificamente nas classes `Pedido` e `Cliente`.

O objetivo destes testes é garantir, de forma isolada e sem dependências externas (como banco de dados ou APIs), que a lógica interna dessas classes se comporta conforme o esperado.

## 2. Estratégia e Escopo

* **Escopo:** A validação se concentra nos métodos `AdicionarItem` da entidade `Pedido` e `ValidarDocumento` da entidade `Cliente`.
* **Tipo de Teste:** Teste de Unidade (automatizado), executado em memória.
* **Estratégia:** Foi utilizado o padrão "Arrange, Act, Assert" (Preparar, Agir, Verificar) para estruturar cada teste. Os cenários cobrem tanto o "caminho feliz" (operações bem-sucedidas) quanto os casos de exceção e dados inválidos.
* **Ferramentas:** xUnit.

## 3. Casos de Teste Detalhados

| ID | Entidade | Descrição do Teste | Cenário | Resultado Esperado | Cobertura da Lógica |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **TU-PED-001** | `Pedido` | Garantir que adicionar um item duplicado apenas incrementa a quantidade. | 1. Um produto é adicionado a um pedido com quantidade 2. <br> 2. O mesmo produto é adicionado novamente com quantidade 3. | A lista de itens do pedido deve conter apenas 1 registro para esse produto, com a quantidade total igual a 5. | Valida a lógica que evita duplicidade de itens e totaliza as quantidades no método `AdicionarItem`. |
| **TU-PED-002** | `Pedido` | Impedir que um produto com estoque zero seja adicionado ao pedido. | 1. Um produto é criado em memória com `QuantidadeEstoque = 0`. <br> 2. O teste tenta adicionar este produto a um novo pedido. | O método `AdicionarItem` deve lançar uma exceção do tipo `InvalidOperationException` com a mensagem "Não é possível adicionar um produto sem estoque.". | Cobre a regra de negócio que verifica a disponibilidade de estoque antes de adicionar um item ao pedido. |
| **TU-CLI-001** | `Cliente` | Validar o formato de documentos (CPF/CNPJ) do cliente. | 1. O teste é executado com uma série de CPFs e CNPJs com formatos válidos (com e sem pontuação). <br> 2. O teste é executado com uma série de documentos inválidos (curtos, com letras, nulos, com dígitos repetidos). | Para os formatos válidos, o método `ValidarDocumento` deve retornar `true`. Para os formatos inválidos, deve retornar `false`. | Garante que a lógica de validação de formato de documentos na entidade `Cliente` está funcionando corretamente para diversos casos. |

## 4. Como Executar os Testes

1.  Navegue até a pasta raiz do projeto pelo terminal.
2.  Execute o comando:
    ```bash
    dotnet test
    ```
Todos os testes de unidade descritos acima devem passar com sucesso.
