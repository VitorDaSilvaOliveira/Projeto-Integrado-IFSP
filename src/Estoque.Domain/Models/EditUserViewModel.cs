using System.ComponentModel.DataAnnotations;
using Estoque.Domain.Enums;

namespace Estoque.Domain.Models;

public class EditUserViewModel
{
    public string Id { get; set; }

    public string? UserName { get; set; }
    
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    public string? AvatarUrl { get; set; }
    public UserStatus Status { get; set; }
}
