# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">

Versão: 1.0
Data: 11 de Novembro de 2025
Autor: Gean Carlos de Sousa Bandeira

## 1. Introdução e Objetivo

Este Pull Request (PR) introduz a suíte inicial e fundamental de testes de unidade para a camada de `Estoque.Domain.Entities`.

O objetivo é garantir que todas as entidades de domínio (como `Cliente`, `Pedido`, `Movimentacao`, etc.) se comportem como esperado, validando duas áreas principais:
1.  **Atribuição de Propriedades:** Assegurar que todas as propriedades (Set/Get) funcionam corretamente.
2.  **Valores Padrão:** Verificar se as entidades são instanciadas com os valores padrão corretos (ex: `Id` como 0, `Status` como Inativo, `string` como `string.Empty` ou `null`, etc.).

## 2. Estratégia e Escopo

* **Escopo:** O escopo deste PR está focado exclusivamente nas classes de entidade puras (POCOs) dentro do namespace `Estoque.Domain.Entities`.
* **Estratégia:** Segue o padrão "Arrange, Act, Assert". Nenhum mock (simulação) é necessário, pois estes testes validam o estado e a construção dos objetos, isolando a lógica do domínio.
* **Ferramentas:** xUnit.

## 3. Casos de Teste Detalhados Adicionados

Foram adicionadas **7 novas classes de teste**, cobrindo 7 entidades de domínio, totalizando **77 novos testes de unidade**.

| Entidade Testada | Descrição do Teste | Total de Testes | Cobertura da Lógica |
| :--- | :--- | :--- | :--- |
| **ClienteTests** | Valida todas as 10 propriedades (Set/Get) e os valores padrão (`Id`, `Nome`, `Status`, `DataCadastro`, etc.). | **17** | Garante a integridade da entidade `Cliente` e seus valores iniciais. |
| **PedidoTests** | Valida propriedades e valores padrão, com foco em tipos anuláveis (como `ClienteId?`, `Status?`, `ValorTotal?`). | **15** | Assegura que o `Pedido` pode ser criado em um estado nulo/padrão antes do preenchimento. |
| **PedidoItemTests** | Valida as propriedades de vínculo (`id_Pedido`, `ProdutoId`) e cálculos (`Quantidade`, `ValorTotal`). | **8** | Cobre o item de linha básico do pedido. |
| **MovimentacaoTests** | Valida propriedades (`IdProduto?`, `DataMovimentacao?`) e o valor padrão do enum `TipoMovimentacao` (Saida = 0). | **14** | Garante que a movimentação de estoque é registrada corretamente. |
| **DevolucaoTests** | Valida as propriedades da devolução (`DataDevolucao`, `Observacao`) e seus valores padrão. | **6** | Cobre a entidade principal de devolução. |
| **DevolucaoItemTests**| Valida as propriedades do item devolvido (`IdProduto`, `QuantidadeDevolvida`). | **7** | Cobre os itens de linha da devolução. |
| **MasterDataTests** | Valida a entidade genérica `MasterData`, incluindo suas propriedades anuláveis (`Json?`, `Sync?`). | **10** | Garante o funcionamento da entidade de metadados. |

## 4. Como Executar os Testes

1.  Navegue até a pasta raiz da solução (ou do projeto de testes) pelo terminal.
2.  Execute o comando:

    ```bash
    dotnet test
    ```
3.  Verifique se todos os 77 testes passam com sucesso.
