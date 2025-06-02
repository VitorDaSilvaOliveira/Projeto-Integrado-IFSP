using System.ComponentModel.DataAnnotations;

namespace Estoque.Domain.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "O usuário é obrigatório")]
    public string Usuario { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória")]
    [DataType(DataType.Password)]
    public string Senha { get; set; }

    [Display(Name = "Lembrar-me")] public bool LembrarMe { get; set; }
}