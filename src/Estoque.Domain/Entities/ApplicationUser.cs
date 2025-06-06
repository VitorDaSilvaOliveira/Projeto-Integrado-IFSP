using Microsoft.AspNetCore.Identity;

namespace Estoque.Domain.Entities;

public class ApplicationUser(string? firstName, string? lastName) : IdentityUser
{
    public string? FirstName { get; set; } = firstName;
    public string? LastName { get; set; } = lastName;
}