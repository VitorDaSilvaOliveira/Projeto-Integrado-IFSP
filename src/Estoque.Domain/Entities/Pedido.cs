using System.ComponentModel.DataAnnotations.Schema;

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
    public int? Status { get; set; }
    public int? Operacao { get; set; }
    public DateTime? DataEntrega { get; set; }
    public string? Observacoes { get; set; }
    public string? UsuarioResponsavel { get; set; }
}