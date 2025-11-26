using System.ComponentModel.DataAnnotations;

namespace Estoque.Domain.Models;

public class ChangeEmailViewModel
{
    public string CurrentEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "O novo email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string NewEmail { get; set; } = string.Empty;
}