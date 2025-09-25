# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">

## 📌 Sobre o Projeto
Sistema completo de gestão de estoque desenvolvido para a **VIP Penha**, loja especializada em eletrônicos. Oferece controle de produtos, movimentações, fornecedores e relatórios integrados.

# Plano de Teste de Integração: Autenticação

**Versão:** 1.0
**Data:** 25 de Setembro de 2025
**Autor:** Gean Carlos de Sousa Bandeira

---

## 1. Introdução e Objetivo

Este documento descreve o plano e o caso de teste de integração para o fluxo de **Recuperação de Senha ("Esqueci minha senha")** do sistema VIP PENHA.

O objetivo deste teste é validar a correta integração entre a camada de serviço de autenticação (`AuthService`) e a camada de dados (Banco de Dados via ASP.NET Identity), garantindo que a funcionalidade de geração de token para reset de senha opera conforme o esperado em um ambiente com dados reais.

## 2. Escopo e Estratégia

* **Escopo:** A funcionalidade de geração de token de reset de senha para um usuário existente.
* **Tipo de Teste:** Teste de Integração (automatizado).
* **Estratégia:** O teste se conecta diretamente ao banco de dados de desenvolvimento para simular um cenário real. Ele busca por um usuário conhecido (`admin`), solicita a geração de um token e valida a integridade do token gerado.

## 3. Pré-condições e Configuração

Para a execução deste teste, as seguintes condições devem ser atendidas:

1.  **Banco de Dados Acessível:** O banco de dados especificado na string de conexão deve estar online e acessível.
2.  **Arquivo de Configuração:** Um arquivo `appsettings.json` deve existir no projeto `Estoque.Tests` com a seguinte estrutura e dados de conexão válidos:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=35.247.225.125,1433;Database=Estoque;User Id=sqlserver;Password=[SUA_SENHA];Encrypt=True;TrustServerCertificate=True;"
      }
    }
    ```
3.  **Usuário de Teste:** Deve existir na tabela `AspNetUsers` do banco de dados um usuário com o `UserName` igual a 'admin'.

## 4. Caso de Teste Detalhado

| ID | Módulo | Descrição do Teste | Passos de Execução | Dados de Teste | Resultado Esperado | Cobertura |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **TI-AUTH-001** | Autenticação | Validar a geração de um token de reset de senha para um usuário existente. | 1. O teste inicializa os serviços de Identity e se conecta ao banco. <br> 2. Busca o usuário pelo `UserName` 'admin'. <br> 3. Solicita ao `UserManager` a geração de um token de reset de senha. <br> 4. Verifica se o token gerado não é nulo ou vazio. <br> 5. (Bônus) Verifica se o token gerado é considerado válido pelo próprio `UserManager`. | `UserName`: "admin" | 1. O usuário 'admin' é encontrado com sucesso no banco. <br> 2. Um token de reset de senha válido (string não vazia) é gerado. <br> 3. O método `VerifyUserTokenAsync` retorna `true`, confirmando a validade do token. | Valida a integração entre o `AuthServiceTests` e o `UserManager` do ASP.NET Identity, cobrindo a funcionalidade principal do fluxo de recuperação de senha e a correta comunicação com o banco de dados. |

## 5. Como Executar

1.  Garanta que as pré-condições acima foram atendidas.
2.  Abra um terminal na pasta raiz do projeto.
3.  Execute o comando:
    ```bash
    dotnet test
    ```
O teste `ForgotPassword_QuandoUsuarioExistente_DeveGerarTokenDeResetValido` deve ser executado e passar com sucesso.
