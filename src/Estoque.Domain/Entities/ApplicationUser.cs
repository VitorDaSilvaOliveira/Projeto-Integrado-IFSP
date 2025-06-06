using Microsoft.AspNetCore.Identity;

namespace Estoque.Domain.Entities;

public class ApplicationUser(string? lastName) : IdentityUser
{
    public string? LastName { get; set; } = lastName;
}