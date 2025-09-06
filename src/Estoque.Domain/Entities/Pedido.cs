using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("Pedido")]
public class Pedido
{
    public int Id { get; set; }
    // Depois
    public string NumeroPedido { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DataPedido { get; set; }
    public int ClienteId { get; set; }
    //public Cliente Cliente { get; set; }
    public decimal ValorTotal { get; set; }

    public DateTime? DataEntrega { get; set; }
    public string Observacoes { get; set; }
    public int? UsuarioResponsavelId { get; set; }
}
