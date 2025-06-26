using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("Produto")]
public class Produto
{
    [Key] [Column("idProduto")] public int IdProduto { get; set; }

    [Column("Nome")] [StringLength(150)] public string? Nome { get; set; }

    [Column("Descricao")]
    [StringLength(200)]
    public string? Descricao { get; set; }

    [Column("Preco", TypeName = "decimal(20,2)")]
    public decimal? Preco { get; set; }

    [Column("QuantidadeEstoque")] public int? QuantidadeEstoque { get; set; }

    [Column("Id_Categoria")] public int? IdCategoria { get; set; }
    [Column("EstoqueMinimo")] public int? EstoqueMinimo { get; set; }
}