// ARQUIVO: DashboardPedidosService.cs (Corrigido para Escopo e Enum)

using System.Globalization;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
// Certifique-se de que esta diretiva use o namespace CORRETO do seu Enum!
using Estoque.Domain.Enums;

namespace Estoque.Infrastructure.Services;

public class DashboardPedidosService(EstoqueDbContext context)
{
    // Mapeamentos de códigos (usando o Enum PedidoStatus)
    private string MapStatus(PedidoStatus? status) => status switch
    {
        null => "Status Não Definido",
        PedidoStatus.EmAndamento => "Em Andamento",
        PedidoStatus.Realizado => "Realizado",
        PedidoStatus.Finalizado => "Finalizado", // CORRIGIDO PARA FINALIZADO
        _ => "Status Desconhecido"
    };

    private string MapOperacao(int operacao) => operacao switch
    {
        1 => "Venda",
        2 => "Troca",
        _ => "Outra Operação"
    };

    // 1. TOP (Operação: Venda/Troca) - Variáveis de escopo REINSERIDAS
    public async Task<(List<string> labels, List<int> vendas, List<int> trocas)> GetPedidosPorOperacaoAsync()
    {
        // VARIÁVEIS DE SCOPE REINSERIDAS AQUI
        var hoje = LocalTime.Now();
        var primeiroMes = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(-3);

        var pedidos = await context.Pedidos
            .Where(p => p.DataPedido != null && p.DataPedido >= primeiroMes)
            .ToListAsync();

        var labels = new List<string>();
        var vendas = new List<int>();
        var trocas = new List<int>();
        // FIM DAS VARIÁVEIS DE SCOPE

        for (var i = 0; i < 4; i++)
        {
            var mesRef = primeiroMes.AddMonths(i);
            var proxMes = mesRef.AddMonths(1);

            vendas.Add(pedidos
                .Where(p => p.Operacao == 1 &&
                            p.DataPedido >= mesRef &&
                            p.DataPedido < proxMes)
                .Count());

            trocas.Add(pedidos
                .Where(p => p.Operacao == 2 &&
                            p.DataPedido >= mesRef &&
                            p.DataPedido < proxMes)
                .Count());

            labels.Add(mesRef.ToString("MMM", new CultureInfo("pt-BR")));
        }

        return (labels, vendas, trocas);
    }

    
    // 2. ESQUERDA BAIXO (Status: 0, 1, 2) - CORRIGIDO
    public async Task<Dictionary<string, int>> GetPedidosPorStatusAsync()
    {
        var dadosAgrupados = await context.Pedidos
            // 1. Agrupar os dados no Banco de Dados (SQL)
            .GroupBy(p => p.Status)
            .Select(g => new
            {
                StatusId = g.Key,
                Quantidade = g.Count()
            })
            // 2. FORÇAR A EXECUÇÃO NO CLIENTE (C#) AQUI!
            // A partir deste ponto, o EF Core não tenta mais traduzir para SQL.
            .ToListAsync();

        // 3. Mapear os Status usando a função C# no lado do Cliente
        var result = dadosAgrupados
            .ToDictionary(
                x => MapStatus(x.StatusId), // Agora a função MapStatus é chamada no C#
                x => x.Quantidade
            );

        // Como a chave do dicionário já é o Status mapeado, não precisamos do .Select(g => new { ... })
        return result;
    }

    // 3. DIREITA BAIXO (Evolução de Faturamento por Mês) - Variáveis de escopo REINSERIDAS
    public async Task<IEnumerable<object>> GetFaturamentoMensalAsync()
    {
        // VARIÁVEIS DE SCOPE REINSERIDAS AQUI
        var hoje = LocalTime.Now();
        var anoAtual = hoje.Year;
        var faturamentoAcumulado = 0m;
        var faturamentoMensal = new List<object>();
        // FIM DAS VARIÁVEIS DE SCOPE

        // CORREÇÃO 2: Se 'Cancelado' não existe, 'Realizado' pode ter um nome diferente ou valor diferente de 1.
        // Se 1 for o valor do enum para Realizado, esta linha está OK.
        var pedidosRealizados = await context.Pedidos
            .Where(p => p.DataPedido != null &&
                        p.DataPedido.Value.Year == anoAtual &&
                        p.Status == PedidoStatus.Realizado)
            .ToListAsync();

        for (var mes = 1; mes <= 12; mes++)
        {
            var mesInicio = new DateTime(anoAtual, mes, 1);
            var mesFim = mesInicio.AddMonths(1);

            var faturamentoDoMes = pedidosRealizados
                .Where(p => p.DataPedido >= mesInicio && p.DataPedido < mesFim)
                .Sum(p => p.ValorTotal ?? 0m);

            faturamentoAcumulado += faturamentoDoMes;

            faturamentoMensal.Add(new
            {
                mes = mesInicio.ToString("MMM", new CultureInfo("pt-BR")),
                valor = faturamentoAcumulado
            });
        }

        return faturamentoMensal;
    }
}