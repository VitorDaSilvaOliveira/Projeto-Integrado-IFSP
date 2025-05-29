using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;
 
[Table("tb_produto")]
public class Produto
{
    [Key] [Column("id_produto")] public int IdProduto { get; set; }

    [Column("nome")] [StringLength(150)] public string? Nome { get; set; }

    [Column("descricao")]
    [StringLength(200)]
    public string? Descricao { get; set; }

    [Column("preco", TypeName = "decimal(20,2)")]
    public decimal? Preco { get; set; }

    [Column("quantidade_estoque")] public int? QuantidadeEstoque { get; set; }

    [Column("id_categoria")] public int? IdCategoria { get; set; }
}