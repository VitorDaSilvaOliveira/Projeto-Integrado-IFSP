using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("DevolucaoItem")]
public class DevolucaoItem
{
    [Key]
    [Column("idDevolucaoItem")]
    public int IdDevolucaoItem { get; set; }

    [Column("IdDevolucao")]
    public int IdDevolucao { get; set; }

    [Column("IdProduto")]
    public int IdProduto { get; set; }

    [Column("IdPedidoItem")]
    public int? IdPedidoItem { get; set; }

    [Column("QuantidadeDevolvida")]
    public int QuantidadeDevolvida { get; set; }

    [Column("Motivo")]
    [StringLength(255)]
    public string? Motivo { get; set; }

    // 🔗 Relacionamentos
    public virtual Devolucao Devolucao { get; set; }
}
