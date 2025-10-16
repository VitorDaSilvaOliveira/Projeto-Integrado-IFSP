

using System.Globalization;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

using Estoque.Domain.Enums;

namespace Estoque.Infrastructure.Services;

public class DashboardPedidosService(EstoqueDbContext context)
{
   
    private string MapStatus(PedidoStatus? status) => status switch
    {
        null => "Status Não Definido",
        PedidoStatus.EmAndamento => "Em Andamento",
        PedidoStatus.Realizado => "Realizado",
        PedidoStatus.Finalizado => "Finalizado", 
        _ => "Status Desconhecido"
    };

    private string MapOperacao(int operacao) => operacao switch
    {
        1 => "Venda",
        2 => "Troca",
        _ => "Outra Operação"
    };

    
    public async Task<(List<string> labels, List<int> vendas, List<int> trocas)> GetPedidosPorOperacaoAsync()
    {
     
        var hoje = LocalTime.Now();
        var primeiroMes = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(-3);

        var pedidos = await context.Pedidos
            .Where(p => p.DataPedido != null && p.DataPedido >= primeiroMes)
            .ToListAsync();

        var labels = new List<string>();
        var vendas = new List<int>();
        var trocas = new List<int>();
        

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

    
    
    public async Task<Dictionary<string, int>> GetPedidosPorStatusAsync()
    {
        var dadosAgrupados = await context.Pedidos
            
            .GroupBy(p => p.Status)
            .Select(g => new
            {
                StatusId = g.Key,
                Quantidade = g.Count()
            })
            
            
            .ToListAsync();

        
        var result = dadosAgrupados
            .ToDictionary(
                x => MapStatus(x.StatusId),
                x => x.Quantidade
            );

        
        return result;
    }


    public async Task<IEnumerable<object>> GetFaturamentoMensalAsync()
    {
        var hoje = LocalTime.Now();
        var anoAtual = hoje.Year;
        var faturamentoAcumulado = 0m;
        var faturamentoMensal = new List<object>();

        
        var pedidosParaFaturar = await context.Pedidos
            .Include(p => p.Itens) 
            .Where(p => p.DataPedido != null &&
                        p.DataPedido.Value.Year == anoAtual &&
                        p.Status == PedidoStatus.Realizado)
            .ToListAsync();

        for (var mes = 1; mes <= 12; mes++)
        {
            var mesInicio = new DateTime(anoAtual, mes, 1);

           
            var pedidosDoMes = pedidosParaFaturar
                .Where(p => p.DataPedido.Value.Month == mes);

         
            var faturamentoDoMes = pedidosDoMes
               
                .SelectMany(p => p.Itens)
              
                .Sum(item => item.PrecoVenda);

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