using System.Globalization;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Services;

public class HomeService(EstoqueDbContext context)
{
    public async Task<Dictionary<string, int>> GetProdutosPorCategoriaAsync()
    {
        var result = await (
            from p in context.Produtos
            join c in context.Categorias on p.IdCategoria equals c.IdCategoria into catGroup
            from c in catGroup.DefaultIfEmpty()
            group p by c != null ? c.NomeCategoria : "Sem Categoria"
            into g
            select new
            {
                Nome = g.Key,
                Quantidade = g.Count()
            }).ToListAsync();

        return result.ToDictionary(x => x.Nome, x => x.Quantidade);
    }

    public async Task<(List<string> labels, List<int> entradas, List<int> saidas)> GetMovimentacoesUltimosMesesAsync()
    {
        var hoje = DateTime.Now;
        var primeiroMes = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(-3);

        var movimentacoes = await context.Movimentacoes
            .Where(m => m.DataMovimentacao != null && m.DataMovimentacao >= primeiroMes)
            .ToListAsync();

        var labels = new List<string>();
        var entradas = new List<int>();
        var saidas = new List<int>();

        for (var i = 0; i < 4; i++)
        {
            var mesRef = primeiroMes.AddMonths(i);
            var proxMes = mesRef.AddMonths(1);

            entradas.Add(movimentacoes
                .Where(m => m.TipoMovimentacao == TipoMovimentacao.Entrada &&
                            m.DataMovimentacao >= mesRef &&
                            m.DataMovimentacao < proxMes)
                .Sum(m => m.Quantidade ?? 0));

            saidas.Add(movimentacoes
                .Where(m => m.TipoMovimentacao == TipoMovimentacao.Saida &&
                            m.DataMovimentacao >= mesRef &&
                            m.DataMovimentacao < proxMes)
                .Sum(m => m.Quantidade ?? 0));

            labels.Add(mesRef.ToString("MMM", new CultureInfo("pt-BR")));
        }

        return (labels, entradas, saidas);
    }

    public async Task<IEnumerable<object>> GetEstoqueMensalAsync()
    {
        var hoje = DateTime.Now;
        var anoAtual = hoje.Year;

        var movimentacoes = await context.Movimentacoes
            .Where(m => m.DataMovimentacao != null && m.DataMovimentacao.Value.Year == anoAtual)
            .ToListAsync();

        var estoqueAcumulado = 0;
        var estoqueMensal = new List<object>();

        for (var mes = 1; mes <= 12; mes++)
        {
            var mesInicio = new DateTime(anoAtual, mes, 1);
            var mesFim = mesInicio.AddMonths(1);

            var entradas = movimentacoes
                .Where(m => m.TipoMovimentacao == TipoMovimentacao.Entrada &&
                            m.DataMovimentacao >= mesInicio &&
                            m.DataMovimentacao < mesFim)
                .Sum(m => m.Quantidade ?? 0);

            var saidas = movimentacoes
                .Where(m => m.TipoMovimentacao == TipoMovimentacao.Saida &&
                            m.DataMovimentacao >= mesInicio &&
                            m.DataMovimentacao < mesFim)
                .Sum(m => m.Quantidade ?? 0);

            estoqueAcumulado += (entradas - saidas);

            estoqueMensal.Add(new
            {
                mes = mesInicio.ToString("MMM", new CultureInfo("pt-BR")),
                quantidade = estoqueAcumulado
            });
        }

        return estoqueMensal;
    }
}