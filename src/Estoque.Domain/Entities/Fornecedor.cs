using System.ComponentModel.DataAnnotations;

namespace Estoque.Domain.Entities;

public class Fornecedor
{
    [Key] public int Id { get; set; }

    [Required] public string RazaoSocial { get; set; } = string.Empty;

    public string? NomeFantasia { get; set; }

    [Required] public string CNPJ { get; set; } = string.Empty;

    public string? InscricaoEstadual { get; set; }

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public string? Rua { get; set; }

    public string? Numero { get; set; }

    public string? Bairro { get; set; }

    public string? Cidade { get; set; }

    public string? Estado { get; set; }

    public string? CEP { get; set; }

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}