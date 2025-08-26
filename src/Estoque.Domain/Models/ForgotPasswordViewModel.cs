using System.ComponentModel.DataAnnotations;

namespace Estoque.Domain.Models;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "O campo E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido")]
    public string Email { get; set; } = null!;
}