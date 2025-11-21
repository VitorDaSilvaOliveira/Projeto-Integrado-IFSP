# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">

# Plano de Testes de Unidade: Cobertura de Domínio e Infraestrutura (Entities, Services & Utils)

**Versão:** 1.0  
**Data:** 20 de Novembro de 2025  
**Autor:** Gean Carlos de Sousa Bandeira

## 1. Introdução e Objetivo

Este Pull Request implementa uma suíte abrangente de testes de unidade visando elevar a confiabilidade do sistema **Estoque**. O foco principal foi cobrir as Entidades de Domínio (garantindo integridade de dados), Classes Utilitárias (lógica pura) e Serviços de Infraestrutura (regras de negócio complexas).

Além da criação de novos testes, este PR resolve conflitos de versionamento do **.NET 10**, corrige duplicidades de pacotes NuGet e ajusta a arquitetura de serviços para permitir injeção de dependência e Mocking eficiente.

## 2. Estratégia e Escopo

* **Escopo:** O trabalho abrangeu `Estoque.Domain` (Entities e Models) e `Estoque.Infrastructure` (Services e Utils).
* **Estratégia:**
    * **Entities:** Validação de propriedades, encapsulamento e valores padrão.
    * **Utils:** Testes de lógica pura para cálculos fiscais (Danfe) e manipulação de objetos.
    * **Services:** Uso de **Mocks (Moq)** para isolar dependências externas (Banco de Dados, `IComponentFactory`).
    * **Integração em Memória:** Uso do `EF Core InMemory` para validar persistência e transações complexas sem afetar o banco real.
* **Ferramentas:** xUnit, Moq, FluentAssertions, Microsoft.EntityFrameworkCore.InMemory.

## 3. Casos de Teste Detalhados e Melhorias

Abaixo, detalhamos as principais implementações e correções realizadas:

| Classe / Componente | Tipo de Teste | Cenário / Descrição | Resultado Esperado |
| :--- | :--- | :--- | :--- |
| **Entities (Cliente, Pedido, Produto, etc.)** | Unidade | Validação de Getters, Setters e Inicialização de Construtores. | Garantir que propriedades (Ids, Enums, Listas) sejam instanciadas corretamente e não nulas quando esperado. |
| **DanfeUtils** | Lógica | Cálculo de Dígito Verificador e Geração de Chave de Acesso NFe (44 dígitos). | A chave gerada deve respeitar o padrão SEFAZ e o cálculo do módulo 11. |
| **ObjectUtils** | Lógica | Extração segura de valores (`SafeGetDecimal`, `SafeGetString`) de objetos anônimos ou nulos. | Retornar valor tipado correto ou valor padrão (0, null) sem lançar exceções. |
| **PedidoService** | Unidade / Mock | Método `ProcessOrder`: Processamento de pedido, cálculo de total e baixa de estoque. | O status do pedido deve mudar para `Realizado`, o valor total calculado e o método de movimentação invocado. |
| **AuditLogService** | Integração (Memória) | Registro de logs de auditoria no banco de dados. | O log deve ser persistido no `DbContext` em memória com IP, Usuário e Ação corretos. |
| **Services (Cliente, Produto)** | Mock Avançado | Método `GetFormView...`: Criação de `JJFormView`. | Simulação da criação de componentes de interface usando `FormatterServices` para contornar construtores complexos. |
| **Correção Infra** | Refatoração | Ajuste em `MovimentacaoService`. | Métodos transformados em `virtual` para permitir sobrescrita e Mocking pelo framework de testes. |
| **Correção Config** | Build | Atualização de `.csproj`. | Unificação das versões de pacotes para `.NET 10.0.0`, eliminando conflitos `NU1504` e `MissingMethodException`. |

---
**Status:** ✅ Todos os testes implementados estão passando (Green).
