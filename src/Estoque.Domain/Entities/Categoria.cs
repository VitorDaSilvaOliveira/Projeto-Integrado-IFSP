using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("tb_categoria")]
public class Categoria
{
    [Key]
    [Column("id_categoria")]
    public int IdCategoria { get; set; }

    [Column("nome")]
    [StringLength(100)]
    public string? Nome { get; set; }

    // Relacionamento com Produto (opcional, pode ser usado em navegação)
    public ICollection<Produto>? Produtos { get; set; }
}
