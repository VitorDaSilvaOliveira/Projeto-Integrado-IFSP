using Estoque.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Estoque.Domain.Entities;

public class ApplicationUser(string? firstName, string? lastName) : IdentityUser
{
    public string? FirstName { get; set; } = firstName;
    public string? LastName { get; set; } = lastName;
    public string? AvatarFileName { get; set; }
    public UserStatus Status { get; set; }
    
}