document.addEventListener("DOMContentLoaded", async function () {

    // 1. DADOS GRÁFICO CENTRAL (Operação: Venda/Troca)
    const operacaoResponse = await fetch('/Estoque/DashboardPedidos/GetPedidosPorOperacao');
    const operacaoData = await operacaoResponse.json();

    // 2. DADOS GRÁFICO ESQUERDO BAIXO (Status: Em Andamento, Realizado, Cancelado)
    const statusResponse = await fetch('/Estoque/DashboardPedidos/GetPedidosPorStatus');
    const statusData = await statusResponse.json();

    // 3. DADOS GRÁFICO DIREITO BAIXO (Faturamento Mensal Acumulado)
    const faturamentoResponse = await fetch('/Estoque/DashboardPedidos/GetFaturamentoMensal');
    const faturamentoMensal = await faturamentoResponse.json();

    // --- GRÁFICO CENTRAL (TOPO): PEDIDOS POR OPERAÇÃO (Barra, como Entradas/Saídas) ---
    new Chart(document.getElementById('operacaoChart').getContext('2d'), {
        type: 'bar',
        data: {
            labels: operacaoData.labels.map(l => l.toUpperCase()),
            datasets: [
                {
                    label: textTranslated.sales, // Ex: "Vendas"
                    data: operacaoData.vendas,
                    backgroundColor: '#3b82f6' // Azul
                },
                {
                    label: textTranslated.exchanges, // Ex: "Trocas"
                    data: operacaoData.trocas,
                    backgroundColor: '#ef4444' // Vermelho
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: { y: { beginAtZero: true } },
            plugins: {
                tooltip: {
                    callbacks: {
                        label: ctx => `${ctx.dataset.label}: ${ctx.raw} ${textTranslated.orders}` // Unidade: Pedidos
                    }
                }
            }
        }
    });

    // --- GRÁFICO ESQUERDO BAIXO: PEDIDOS POR STATUS (Pizza) ---
    new Chart(document.getElementById('statusChart').getContext('2d'), {
        type: 'pie',
        data: {
            labels: Object.keys(statusData.status),
            datasets: [{
                data: Object.values(statusData.status),
                backgroundColor: ['#10b981', '#6366f1', '#f59e0b', '#ef4444', '#8b5cf6', '#14b8a6']
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });

    // --- GRÁFICO DIREITO BAIXO: FATURAMENTO MENSAL (Linha, como Estoque Mensal) ---
    const labelsReal = faturamentoMensal.map(x => x.mes.toUpperCase());
    const valoresFaturamento = faturamentoMensal.map(x => x.valor);
    const menorValor = Math.min(...valoresFaturamento);
    const maiorValor = Math.max(...valoresFaturamento);

    new Chart(document.getElementById('faturamentoMensalChart').getContext('2d'), {
        type: 'line',
        data: {
            labels: labelsReal,
            datasets: [{
                label: textTranslated.totalRevenue,
                data: valoresFaturamento,
                borderColor: '#0ea5e9',
                backgroundColor: 'rgba(14, 165, 233, 0.2)',
                fill: true,
                tension: 0.4,
                pointRadius: 4,
                pointBackgroundColor: '#0ea5e9'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    min: Math.max(0, menorValor - 0.1 * maiorValor),
                    max: maiorValor + 0.1 * maiorValor,
                    ticks: {
                        // Formatação para Moeda
                        callback: (value) => `${textTranslated.currency} ${value.toFixed(2).replace('.', ',')}`
                    }
                }
            },
            plugins: {
                tooltip: {
                    callbacks: {
                        // Tooltip mostrando valor em moeda
                        label: ctx => `${ctx.dataset.label}: ${textTranslated.currency} ${ctx.raw.toFixed(2).replace('.', ',')}`
                    }
                }
            }
        }
    });
});