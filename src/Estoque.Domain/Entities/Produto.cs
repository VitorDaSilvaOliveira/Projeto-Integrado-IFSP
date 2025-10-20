using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("Produto")]
public class Produto
{
    [Key] [Column("idProduto")] public int IdProduto { get; set; }

    [Column("Nome")] [StringLength(150)] public string? Nome { get; set; }
    [Column("Codigo")] [StringLength(150)] public string? Codigo { get; set; }

    [Column("Descricao")]
    [StringLength(200)]
    public string? Descricao { get; set; }

    [Column("Preco", TypeName = "decimal(20,2)")]
    public decimal? Preco { get; set; }

    [Column("Rastreio")] public bool? RastrearPorSerie { get; set; }

    [Column("Id_Categoria")] public int? IdCategoria { get; set; }
   // [ForeignKey("IdCategoria")]
    [Column("EstoqueMinimo")] public int? EstoqueMinimo { get; set; }
    
    [Column("DiasGarantia")] public int? DiasGarantia { get; set; }
    
    [ForeignKey("IdCategoria")]
    public virtual Categoria? Categoria { get; set; } 

    public virtual ICollection<ProdutoLote> ProdutoLotes { get; set; } = new List<ProdutoLote>();
}