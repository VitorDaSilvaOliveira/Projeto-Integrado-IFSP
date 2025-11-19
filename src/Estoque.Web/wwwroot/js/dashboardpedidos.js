document.addEventListener("DOMContentLoaded", carregarDashboard);

async function carregarDashboard() {
    try {
        const resp = await fetch("/Estoque/Pedido/GetDashboardData");
        if (!resp.ok) throw new Error("Erro ao carregar dados.");

        const data = await resp.json();

        document.getElementById("kpiTotal").innerText = data.totalPedidos;
        document.getElementById("kpiValor").innerText = data.valorTotal.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        document.getElementById("kpiTicket").innerText = data.ticketMedio.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        document.getElementById("kpiHoje").innerText = data.pedidosHoje;

        montarGraficoOperacao(data.pedidosPorOperacao);
        montarGraficoStatus(data.pedidosPorStatus);
        montarGraficoDia(data.pedidosPorDia);

    } catch (err) {
        console.error("Erro no dashboard:", err);
    }
}

function montarGraficoOperacao(dados) {
    new Chart(document.getElementById("chartOperacao"), {
        type: "bar",
        data: {
            labels: dados.map(x => x.operacao),
            datasets: [{
                label: "Total",
                data: dados.map(x => x.total),
                backgroundColor: "rgba(54, 162, 235, 0.5)",
                borderColor: "rgba(54, 162, 235, 1)",
                borderWidth: 1
            }]
        }
    });
}

function montarGraficoStatus(dados) {
    new Chart(document.getElementById("chartStatus"), {
        type: "pie",
        data: {
            labels: dados.map(x => x.status),
            datasets: [{
                data: dados.map(x => x.total),
                backgroundColor: [
                    "rgba(75, 192, 192, 0.5)",
                    "rgba(255, 159, 64, 0.5)",
                    "rgba(255, 99, 132, 0.5)"
                ]
            }]
        }
    });
}

function montarGraficoDia(dados) {
    new Chart(document.getElementById("chartDia"), {
        type: "line",
        data: {
            labels: dados.map(x => new Date(x.dia).toLocaleDateString("pt-BR")),
            datasets: [{
                label: "Pedidos por Dia",
                data: dados.map(x => x.total),
                borderColor: "rgba(153, 102, 255, 1)",
                tension: 0.3
            }]
        }
    });
}