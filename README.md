# Plano de Testes - Lógica de Domínio

Este documento detalha o plano de testes para validação das regras de negócio contidas diretamente nas entidades de domínio do projeto, focando na classe `Produto`.

## 1. Estratégia e Escopo

A estratégia adotada foi o **Teste de Unidade** focado na entidade `Produto` para garantir que suas regras de negócio internas são robustas e corretas. O escopo é a validação da lógica de manipulação de estoque.

-   **Ferramentas:** xUnit, .NET
-   **Técnica:** Teste funcional de caixa-preta (positivo e negativo).

## 2. Plano de Testes Detalhado

| Descrição                                                     | Tipo de Teste     | Dados de Teste                                                      | Resultado Esperado                                                                                             | Cobertura do Código                                                                    |
| :-------------------------------------------------------------- | :---------------- | :------------------------------------------------------------------ | :--------------------------------------------------------------------------------------------------------------- | :------------------------------------------------------------------------------------- |
| **Garantir que a baixa de estoque funciona com valores válidos.** | **Teste de Unidade** | `QuantidadeEstoque` inicial: 10<br>`QuantidadeParaBaixa`: 3 | O `QuantidadeEstoque` final do produto deve ser 7.                                                           | Cobre o fluxo principal do método `DarBaixaEstoque`.                                   |
| **Impedir que a baixa de estoque deixe a quantidade negativa.** | **Teste de Unidade** | `QuantidadeEstoque` inicial: 5<br>`QuantidadeParaBaixa`: 8  | O método deve lançar uma exceção `InvalidOperationException` com a mensagem "Estoque insuficiente para realizar a baixa.". | Cobre a lógica de validação que impede o estoque de ficar negativo.                    |
| **Impedir que a baixa de estoque seja feita com um número negativo.** | **Teste de Unidade** | `QuantidadeEstoque` inicial: 20<br>`QuantidadeParaBaixa`: -5 | O método deve lançar uma exceção `ArgumentException` com a mensagem "A quantidade para baixa não pode ser negativa.". | Cobre a validação de dados de entrada, garantindo que apenas valores positivos são aceitos. |

## 3. Como Executar os Testes

```bash
dotnet test
