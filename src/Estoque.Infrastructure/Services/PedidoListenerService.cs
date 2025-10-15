using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Estoque.Infrastructure.Services
{
public class PedidoListenerService : BackgroundService
{
    private readonly IServiceProvider _services;

    public PedidoListenerService(IServiceProvider services)
    {
        _services = services;
    }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();

                    var novosEventos = db.Eventos
                        .Where(e => !e.Processado)
                        .ToList();

                    foreach (var evento in novosEventos)
                    {
                        await ConfereEnvioPedido(evento);

                        evento.Processado = true;
                    }

                    await db.SaveChangesAsync();
                }

                await Task.Delay(5000, stoppingToken); // checa a cada 5s
            }
        }

        private async Task ConfereEnvioPedido(Evento evento)
        {
            if (evento.Campo == "Status" && evento.ValorAntigo == "0" && evento.ValorNovo == "1")
            {
                Console.WriteLine($"➡️ O status mudou de {evento.ValorAntigo} para {evento.ValorNovo}");

                //await GerarMovimentacoesPedido(evento.IdRegistro);
            }

            return;
        }

        private async Task GerarMovimentacoesPedido(int? idPedido)
        {
            if (idPedido == null) return;

            using (var scope = _services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
                var movService = scope.ServiceProvider.GetRequiredService<MovimentacaoService>();

                var itens = db.PedidosItens.Where(i => i.id_Pedido == idPedido).ToList();

                foreach (var item in itens)
                {
                    await movService.RegistrarMovimentacaoAsync(
                        produtoId: item.ProdutoId,
                        quantidade: -item.Quantidade,
                        tipoMovimentacao: TipoMovimentacao.Saida,
                        userId: null,
                        observacao: $"Saída automática de estoque para o Pedido {idPedido}."// Argumento 5 (string)
                       // pedidoId: idPedido
                    );
                }
            }
        }



    }
}
