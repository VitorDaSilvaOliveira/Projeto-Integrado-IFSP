namespace Estoque.Domain.Models;

public class EditRoleViewModel
{
    public string RoleId { get; set; } = default!;
    public string RoleName { get; set; } = default!;
    public List<UserRoleViewModel> Users { get; set; } = new();
}

public class UserRoleViewModel
{
    public string UserId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public bool IsSelected { get; set; }
}
