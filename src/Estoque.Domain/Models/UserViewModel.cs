using System.ComponentModel.DataAnnotations;

namespace Estoque.Domain.Models;

public class UserViewModel
{
    [Required(ErrorMessage = "O campo Nome de Usuário é obrigatório.")]
    public string Username { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Phone]
    public string PhoneNumber { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}
