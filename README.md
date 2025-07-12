# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">

## 📌 Sobre o Projeto
Sistema completo de gestão de estoque desenvolvido para a **VIP Penha**, loja especializada em eletrônicos. Oferece controle de produtos, movimentações, fornecedores e relatórios integrados.

# Plano de Testes - Projeto Integrado (Sistema de Estoque)

Este documento descreve o plano de testes, as estratégias, as técnicas e os casos de teste aplicados ao projeto do Sistema de Estoque, em conformidade com os requisitos da disciplina de Engenharia de Software IV.

## 1. Introdução e Escopo

O objetivo destes testes é assegurar a robustez e a corretude da lógica de negócio do sistema, focando na validação e criação de instâncias da entidade `Produto` através da classe `ProdutoService`.

-   **Escopo dos Testes:** A lógica de negócio contida no método `CriarNovoProduto` da classe `ProdutoService`, responsável pela validação de dados e instanciação de objetos `Produto`.
-   **Fora do Escopo:** Testes de interface de usuário (UI/Front-end), testes de performance, testes de segurança e a camada de acesso a dados (interação direta com o banco de dados).

## 2. Estratégia e Ferramentas

### 2.1. Tipos de Testes Aplicados

-   **Testes de Unidade:** Utilizados para isolar e verificar as menores unidades de código — neste caso, o método `CriarNovoProduto`. Eles validam as regras de negócio de forma rápida e eficiente, garantindo que o método se comporta como esperado sob diferentes condições.

### 2.2. Técnicas de Teste Empregadas

-   **Caixa-Preta:** Os testes foram projetados sem conhecimento da implementação interna do método, focando apenas nas entradas e saídas esperadas.
-   **Teste Funcional (Positivo e Negativo):** Validamos tanto o "caminho feliz" (entradas válidas) quanto os cenários de erro (entradas inválidas).
-   **Análise de Valor Limite:** Empregamos esta técnica para testar os valores nas fronteiras das regras de negócio (por exemplo, `quantidade = 0`).

### 2.3. Ferramentas Utilizadas

-   **Framework de Teste:** xUnit
-   **Plataforma:** .NET
-   **Linguagem:** C#

## 3. Plano de Testes Detalhado

A tabela a seguir detalha os 3 casos de teste executáveis que foram implementados.

| Descrição                                             | Tipo de Teste     | Técnica(s) Aplicada(s)                           | Pré-condições                        | Passos para Execução                                                                                            | Dados de Teste                                                                                             | Resultado Esperado                                                                                                         | Cobertura do Código                                                                                         |
| :------------------------------------------------------ | :---------------- | :----------------------------------------------- | :----------------------------------- | :-------------------------------------------------------------------------------------------------------------- | :--------------------------------------------------------------------------------------------------------- | :------------------------------------------------------------------------------------------------------------------------- | :------------------------------------------------------------------------------------------------------------------------ |
| Verificar a criação de um produto com dados válidos.    | **Teste de Unidade** | Teste Funcional Positivo (Caminho Feliz)         | Instância do `ProdutoService` criada. | 1. Chamar o método `CriarNovoProduto`. <br> 2. Passar parâmetros válidos para nome, descrição, preço e quantidade. <br> 3. Verificar o objeto retornado. | `nome`: "Notebook Gamer Dell G15" <br> `descricao`: "i7, 16GB RAM, RTX 3060" <br> `preco`: 7500.50 <br> `quantidade`: 15 | O método deve retornar uma instância válida e não nula da classe `Produto`, com suas propriedades preenchidas com os dados de entrada. | Cobre o fluxo de execução principal do método `CriarNovoProduto`, onde todas as validações são bem-sucedidas.                |
| Impedir a criação de um produto com preço negativo.      | **Teste de Unidade** | Teste Funcional Negativo (Tratamento de Erro)    | Instância do `ProdutoService` criada. | 1. Chamar `CriarNovoProduto`. <br> 2. Passar um valor de preço negativo. <br> 3. Capturar e verificar a exceção lançada. | `nome`: "Cadeira Gamer" <br> `descricao`: "Cadeira ergonômica" <br> `preco`: -200.00 <br> `quantidade`: 10         | O sistema deve lançar uma exceção do tipo `ArgumentException` contendo a mensagem exata: "O preço do produto não pode ser negativo.". | Cobre o bloco de validação de preço (`if (preco < 0)`), garantindo que a regra de negócio de preço não negativo seja aplicada. |
| Permitir a criação de um produto com quantidade zero.   | **Teste de Unidade** | Análise de Valor Limite                          | Instância do `ProdutoService` criada. | 1. Chamar `CriarNovoProduto`. <br> 2. Passar `0` como valor para a quantidade. <br> 3. Verificar se o objeto é criado com sucesso. | `nome`: "Webcam Logitech C920" <br> `descricao`: "Full HD 1080p" <br> `preco`: 450.00 <br> `quantidade`: 0        | O método deve executar sem erros e retornar uma instância válida de `Produto`, com a propriedade `QuantidadeEstoque` igual a 0. | Cobre o caso limite da regra de negócio `quantidade >= 0`, validando que o valor 0 é um estado aceitável para o sistema.  |

## 4. Como Executar os Testes

1.  Clone o repositório e acesse a branch `test-plan-implementation`.
2.  Abra um terminal na pasta raiz do projeto.
3.  Execute o seguinte comando para rodar todos os testes da solução:

    ```bash
    dotnet test
    ```

A saída no terminal confirmará o resultado da execução. O resultado esperado é:
