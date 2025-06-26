document.addEventListener("DOMContentLoaded", function () {
    fetch('/Estoque/Notificacao/GetNotificacoes')
        .then(res => res.text())
        .then(html => {
            const container = document.getElementById('notificacoes-container');
            if (container) container.innerHTML = html;
        });
});