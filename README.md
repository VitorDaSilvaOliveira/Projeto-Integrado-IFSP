# Sistema de Controle de Estoque VIP Penha <img src="src/Estoque.Web/wwwroot/img/logo.png" alt="Vip-Penha Logo" width="50" height="50">


## Testes e Qualidade de Código

**Tecnologias:**
- **xUnit** (Framework de Testes)
- **Moq** (Simulação de dependências e Banco de Dados)
- **FluentAssertions** (Asserções mais legíveis)

**Cobertura Principal:**
- ✅ **Estoque & Lotes:** Validação rigorosa da lógica FIFO/LIFO e baixa de estoque.
- ✅ **Vendas:** Fluxo completo de criação e processamento de pedidos.
- ✅ **Infraestrutura:** Garantia de gravação de Logs de Auditoria e configuração de serviços.
- ✅ **Domínio:** Validação de integridade das entidades principais.

**Como rodar os testes:**
```bash
dotnet test
