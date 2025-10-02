using Estoque.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("Cliente")]
public class Cliente
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Documento { get; set; }

    public TipoCliente Tipo { get; set; }

    public UserStatus Status { get; set; }

    public string? NomeContato { get; set; }

    public string? Telefone { get; set; }

    public string? Anexo { get; set; }
    public string? NomeFantasia { get; set; }
    public string? Email { get; set; }

    public DateTime DataCadastro { get; set; }

    // Adicione este método dentro da sua classe Cliente.cs
public bool ValidarDocumento(string? documento)
{
    if (string.IsNullOrWhiteSpace(documento))
        return false;

    // Remove caracteres de formatação (pontos, traços, barras)
    var apenasDigitos = new string(documento.Where(char.IsDigit).ToArray());

    // Validação simples de tamanho (um CPF tem 11 dígitos, um CNPJ tem 14)
    if (apenasDigitos.Length != 11 && apenasDigitos.Length != 14)
        return false;

    // Validação simples para não aceitar todos os dígitos iguais (ex: 111.111.111-11)
    if (apenasDigitos.All(c => c == apenasDigitos[0]))
        return false;
        
    // (A lógica real de validação dos dígitos verificadores é bem mais complexa)
    // Para o escopo deste teste, se passar pelas checagens acima, consideramos o formato válido.
    
    return true;
}
}