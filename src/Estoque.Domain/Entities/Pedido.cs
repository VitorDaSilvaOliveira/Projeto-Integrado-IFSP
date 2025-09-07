using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("Pedido")]
public class Pedido
{
    public int Id { get; set; }
    public string NumeroPedido { get; set; }
    public DateTime DataPedido { get; set; }
    public int ClienteId { get; set; }
    //public Cliente Cliente { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; }
    //public ICollection<PedidoItem> Itens { get; set; }
    public DateTime? DataEntrega { get; set; }
    public string Observacoes { get; set; }
    public int? UsuarioResponsavelId { get; set; }
}