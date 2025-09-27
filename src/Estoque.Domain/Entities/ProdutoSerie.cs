using Estoque.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("ProdutoSerie")]
public class ProdutoSerie
{
    [Key]
    public int SerieId { get; set; }

    [Required]
    public int LoteId { get; set; }

    [Required]
    public string NumeroSerie { get; set; } = string.Empty;

    [Required]
    public StatusProdutoSerie Status { get; set; } = StatusProdutoSerie.EmEstoque;

    [ForeignKey("LoteId")]
    public virtual ProdutoLote ProdutoLote { get; set; }
}
