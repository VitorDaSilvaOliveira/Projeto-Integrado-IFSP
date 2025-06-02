using System.ComponentModel.DataAnnotations;

namespace Estoque.Domain.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "O campo Login é obrigatório")]
    public string Login { get; set; }

    [Required(ErrorMessage = "O campo Senha é obrigatório")]
    [DataType(DataType.Password)]
    public string Senha { get; set; }

    [Display(Name = "Lembrar-me")] public bool LembrarMe { get; set; }
}