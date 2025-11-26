using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Repository;

public class DevolucaoRepository(EstoqueDbContext context)
{
    public async Task<Devolucao?> GetDevolucaoAsync(int devolucaoId)
    {
        var itens =
            from item in context.DevolucoesItens
            join produto in context.Produtos
                on item.IdProduto equals produto.IdProduto
            where item.IdDevolucao == devolucaoId
            select new DevolucaoItem
            {
                IdDevolucaoItem = item.IdDevolucaoItem,
                IdDevolucao = item.IdDevolucao,
                IdProduto = item.IdProduto,
                IdPedidoItem = item.IdPedidoItem,
                QuantidadeDevolvida = item.QuantidadeDevolvida,
                Motivo = item.Motivo,
                Devolvido = item.Devolvido,

                Produto = new Produto
                {
                    IdProduto = produto.IdProduto,
                    Nome = produto.Nome,
                    Codigo = produto.Codigo,
                    Descricao = produto.Descricao
                }
            };

        var devolucao = await context.Devolucoes
            .FirstOrDefaultAsync(x => x.IdDevolucao == devolucaoId);

        if (devolucao != null)
            devolucao.Itens = await itens.ToListAsync();

        return devolucao;
    }

    public async Task<List<DevolucaoItem>> GetItensAsync(int devolucaoId)
    {
        var query =
            from item in context.DevolucoesItens
            join produto in context.Produtos
                on item.IdProduto equals produto.IdProduto
            where item.IdDevolucao == devolucaoId
            select new DevolucaoItem
            {
                IdDevolucaoItem = item.IdDevolucaoItem,
                IdDevolucao = item.IdDevolucao,
                IdProduto = item.IdProduto,
                IdPedidoItem = item.IdPedidoItem,
                QuantidadeDevolvida = item.QuantidadeDevolvida,
                Motivo = item.Motivo,
                Devolvido = item.Devolvido,

                // 🔥 Propriedade "produto" dinâmica
                // sem precisar existir na entity
                Produto = new Produto
                {
                    IdProduto = produto.IdProduto,
                    Nome = produto.Nome,
                    Codigo = produto.Codigo
                }
            };

        return await query.ToListAsync();
    }


    public Task<int> SaveAsync()
    {
        return context.SaveChangesAsync();
    }

    //public async Task<object> DashboardAsync()
    //{
    //    var hoje = DateTime.Today;
    //    var devolucoes = context.Devolucoes.AsQueryable();

    //    var totalPedidos = await devolucoes.CountAsync();
    //    var valorTotal = await devolucoes.SumAsync(p => p.ValorTotal) ?? 0;
    //    var pedidosHoje = await devolucoes.CountAsync(p => p.DataPedido >= hoje);
    //    var ticketMedio = totalPedidos > 0 ? valorTotal / totalPedidos : 0;

    //    var porOperacao = await devolucoes
    //        .Where(p => p.Operacao != null)
    //        .GroupBy(p => p.Operacao)
    //        .Select(g => new
    //        {
    //            Operacao = g.Key.ToString(),
    //            Total = g.Count()
    //        })
    //        .ToListAsync();

    //    var porStatus = await devolucoes
    //        .Where(p => p.Status != null)
    //        .GroupBy(p => p.Status)
    //        .Select(g => new
    //        {
    //            Status = g.Key.ToString(),
    //            Total = g.Count()
    //        })
    //        .ToListAsync();

    //    var porDia = await devolucoes
    //        .Where(p => p.DataPedido != null)
    //        .GroupBy(p => p.DataPedido!.Value.Date)
    //        .Select(g => new
    //        {
    //            Dia = g.Key,
    //            Total = g.Count()
    //        })
    //        .OrderBy(x => x.Dia)
    //        .ToListAsync();

    //    return new
    //    {
    //        TotalPedidos = totalPedidos,
    //        ValorTotal = valorTotal,
    //        TicketMedio = ticketMedio,
    //        PedidosHoje = pedidosHoje,
    //        PedidosPorOperacao = porOperacao,
    //        PedidosPorStatus = porStatus,
    //        PedidosPorDia = porDia
    //    };
    //}
}