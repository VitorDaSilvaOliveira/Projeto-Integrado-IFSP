namespace Estoque.Domain.Models;

public class EditRoleViewModel
{
    public string RoleId { get; set; } = default!;
    public string RoleName { get; set; } = default!;

    public List<EditMenuViewModel> Menus { get; set; } = new();

    public List<UserRoleViewModel> Users { get; set; } = new();
}

public class UserRoleViewModel
{
    public string UserId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public bool IsSelected { get; set; }
}

public class EditMenuViewModel
{
    public string MenuId { get; set; }
    public string MenuName { get; set; }
    public bool IsSelected { get; set; }
}