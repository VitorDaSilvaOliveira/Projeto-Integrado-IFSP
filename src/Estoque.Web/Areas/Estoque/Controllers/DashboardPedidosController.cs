using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Estoque.Controllers;

[Area("Estoque")]
public class DashboardPedidosController(DashboardPedidosService pedidosService) : Controller
{
    public IActionResult Index() => View();

    // 1. TOP (Operação: Venda/Troca)
    [HttpGet]
    public async Task<IActionResult> GetPedidosPorOperacao()
    {
        var (labels, vendas, trocas) = await pedidosService.GetPedidosPorOperacaoAsync();
        // Retorna dados separados para o gráfico de Barra, semelhante ao 'Entradas' e 'Saídas' da Home.
        return Json(new { labels, vendas, trocas });
    }

    // 2. ESQUERDA BAIXO (Status: 0, 1, 2)
    [HttpGet]
    public async Task<IActionResult> GetPedidosPorStatus()
    {
        var data = await pedidosService.GetPedidosPorStatusAsync();
        // Retorna um Dictionary para o gráfico de Pizza.
        return Json(new { status = data });
    }

    // 3. DIREITA BAIXO (Evolução de Faturamento por Mês)
    [HttpGet]
    public async Task<IActionResult> GetFaturamentoMensal()
    {
        var data = await pedidosService.GetFaturamentoMensalAsync();
        // Retorna uma lista de objetos (Mês e Valor Acumulado) para o gráfico de Linha.
        return Json(data);
    }
}