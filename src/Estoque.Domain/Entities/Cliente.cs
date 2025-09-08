using Estoque.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("Cliente")]
public class Cliente
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Documento { get; set; }
    public TipoCliente Tipo { get; set; }
    public UserStatus Status { get; set; }
    public string? NomeContato { get; set; }
    public string? Telefone { get; set; }
    public string? Anexo { get; set; }
    public DateTime DataCadastro { get; set; }
}