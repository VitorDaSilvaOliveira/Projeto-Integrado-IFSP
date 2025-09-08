using Estoque.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("Cliente")]
public class Cliente
{
    [Key]
    [Column("IdCliente")]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Documento { get; set; }

    [Required]
    public TipoCliente Tipo { get; set; }

    [Required]
    public UserStatus Status { get; set; }

    [StringLength(150)]
    public string? NomeContato { get; set; }

    [StringLength(20)]
    public string? Telefone { get; set; }

    [StringLength(255)]
    public string? Anexo { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}