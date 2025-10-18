document.addEventListener("DOMContentLoaded", async () => {
    const response = await fetch("/Estoque/Pedido/GetPedidosPorOperacao"); 

    if (!response.ok) {
        console.error("Erro ao buscar dados:", response.statusText);
        return;
    }

    const text = await response.text();
    if (!text) {
        console.warn("Resposta vazia do servidor");
        return;
    }

    const data = JSON.parse(text);

    const labels = data.map(x => x.operacao);
    const valores = data.map(x => x.total);

    const ctx = document.getElementById("graficoPedidos").getContext("2d");
    new Chart(ctx, {
        type: "bar",
        data: {
            labels,
            datasets: [{
                label: "Pedidos por Operação",
                data: valores,
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: { beginAtZero: true }
            }
        }
    });
});
