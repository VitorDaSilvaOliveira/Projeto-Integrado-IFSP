using System.ComponentModel.DataAnnotations.Schema;
using Estoque.Domain.Enums;

namespace Estoque.Domain.Entities;

[Table("Pedido")]
public class Pedido
{
    public int Id { get; set; }
    public string? NumeroPedido { get; set; }
    public DateTime? DataPedido { get; set; }
    [Column("Cliente_Id")]
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public decimal? ValorTotal { get; set; }
    public PedidoStatus? Status { get; set; }
    public PedidoOperacao? Operacao { get; set; }
    public DateTime? DataEntrega { get; set; }
    public string? Observacoes { get; set; }
    public string? UsuarioResponsavel { get; set; }

    public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();

}