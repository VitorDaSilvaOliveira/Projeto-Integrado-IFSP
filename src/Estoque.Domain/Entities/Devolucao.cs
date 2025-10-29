using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("Devolucao")]
public class Devolucao
{
    [Key]
    [Column("idDevolucao")]
    public int IdDevolucao { get; set; }

    [Column("TipoDevolucao")]
    public byte TipoDevolucao { get; set; }

    [Column("DataDevolucao", TypeName = "date")]
    public DateTime DataDevolucao { get; set; }

    [Column("Observacao")]
    [StringLength(255)]
    public string? Observacao { get; set; }

    [Column("Id_User")]
    [StringLength(100)]
    public string IdUser { get; set; }

    [Column("Devolvido")]
    public byte? Devolvido { get; set; }

    public virtual ICollection<DevolucaoItem> Itens { get; set; }
}
