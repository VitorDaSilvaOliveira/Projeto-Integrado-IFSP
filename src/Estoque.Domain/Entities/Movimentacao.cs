using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Estoque.Domain.Enums;

namespace Estoque.Domain.Entities;

[Table("Movimentacao")]
public class Movimentacao
{
    [Key]
    [Column("idMovimentacao")]
    public int IdMovimentacao { get; set; }

    [Column("Id_Produto")]
    public int? IdProduto { get; set; }

    [ForeignKey("IdProduto")]
    public Produto? Produto { get; set; }

    [Column("Quantidade")]
    public int? Quantidade { get; set; }

    [Column("TipoMovimentacao")]
    public TipoMovimentacao TipoMovimentacao { get; set; }

    [Column("DataMovimentacao", TypeName = "date")]
    public DateTime? DataMovimentacao { get; set; }

    [Column("Observacao")]
    [StringLength(255)]
    public string? Observacao { get; set; }

    [Column("Id_User")]
    [StringLength(100)]
    public string? IdUser { get; set; }
}