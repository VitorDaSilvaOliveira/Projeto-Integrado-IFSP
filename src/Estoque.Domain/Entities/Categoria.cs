using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("Categoria")]
public class Categoria
{
    [Key] [Column("idCategoria")] public int IdCategoria { get; set; }

    [Column("NomeCategoria")] [StringLength(100)] public string? NomeCategoria { get; set; }
}