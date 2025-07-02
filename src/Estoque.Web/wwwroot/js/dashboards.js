document.addEventListener("DOMContentLoaded", async function () {
    const categoriaResponse = await fetch('/Estoque/Home/GetProdutosPorCategoria');
    const categoriaData = await categoriaResponse.json();

    const movResponse = await fetch('/Estoque/Home/GetMovimentacoesUltimosMeses');
    const movData = await movResponse.json();

    new Chart(document.getElementById('estoqueChart').getContext('2d'), {
        type: 'bar',
        data: {
            labels: movData.labels.map(l => l.toUpperCase()),
            datasets: [
                {
                    label: textTranslated.entries,
                    data: movData.entradas,
                    backgroundColor: '#3b82f6'
                },
                {
                    label: textTranslated.exits,
                    data: movData.saidas,
                    backgroundColor: '#ef4444'
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {y: {beginAtZero: true}},
            plugins: {
                tooltip: {
                    callbacks: {
                        label: ctx => `${ctx.dataset.label}: ${ctx.raw} ${textTranslated.units}`
                    }
                }
            }
        }
    });

    new Chart(document.getElementById('pizzaChart').getContext('2d'), {
        type: 'pie',
        data: {
            labels: Object.keys(categoriaData.produtosPorCategoria),
            datasets: [{
                data: Object.values(categoriaData.produtosPorCategoria),
                backgroundColor: ['#6366f1', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#14b8a6']
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });

    const estoqueResponse = await fetch('/Estoque/Home/GetEstoqueMensal');
    const estoqueMensal = await estoqueResponse.json();

    const labelsReal = estoqueMensal.map(x => x.mes.toUpperCase());
    const valoresEstoque = estoqueMensal.map(x => x.quantidade);
    const menorValor = Math.min(...valoresEstoque);
    const maiorValor = Math.max(...valoresEstoque);

    new Chart(document.getElementById('estoqueMensalChart').getContext('2d'), {
        type: 'line',
        data: {
            labels: labelsReal,
            datasets: [{
                label: textTranslated.totalStock,
                data: valoresEstoque,
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
                    min: Math.max(0, menorValor - 40),
                    max: maiorValor + 40,
                    ticks: {stepSize: 50}
                }
            }
        }
    });
});