async function loadDashboard() {
    try {
        const resp = await fetch('/Estoque/Cliente/GetDashboardData');
        if (!resp.ok) throw new Error("Falha na API");

        const d = await resp.json();

        document.getElementById("totalClientes").innerText = d.totalClientes;
        document.getElementById("ativos").innerText = d.ativos;
        document.getElementById("inativos").innerText = d.inativos;

        const tbody = document.querySelector("#tblTopClientes tbody");
        tbody.innerHTML = "";

        if (d.topClientes.length === 0) {
            tbody.innerHTML = '<tr><td colspan="3" class="text-center">Nenhum dado</td></tr>';
            return;
        }

        d.topClientes.forEach(c => {
            const row = `
                        <tr>
                            <td>${c.nomeCliente}</td>
                            <td>${c.quantidadePedidos}</td>
                            <td>R$ ${c.totalGasto.toLocaleString("pt-BR")}</td>
                        </tr>`;
            tbody.insertAdjacentHTML("beforeend", row);
        });

    } catch (e) {
        console.error(e);
        alert("Erro ao carregar dashboard.");
    }
}

loadDashboard();