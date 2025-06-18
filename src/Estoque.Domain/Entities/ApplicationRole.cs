using Microsoft.AspNetCore.Identity;

namespace Estoque.Domain.Entities;

public class ApplicationRole : IdentityRole
{
    public DateTime CreationDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }

    public ApplicationRole() : base() { }

    public ApplicationRole(string roleName) : base(roleName)
    {
        CreationDate = DateTime.UtcNow;
    }
}
