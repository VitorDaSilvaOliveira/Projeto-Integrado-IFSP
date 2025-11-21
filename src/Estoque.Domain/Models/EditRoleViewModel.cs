namespace Estoque.Domain.Models;

public class EditRoleViewModel
{
    public string RoleId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public List<EditMenuViewModel> Menus { get; set; } = [];
    public List<EditMenuGroupViewModel> Groups { get; set; } = [];
    public List<UserRoleViewModel> Users { get; set; } = [];
}

public class UserRoleViewModel
{
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public bool IsSelected { get; set; }
}

public class EditMenuViewModel
{
    public string MenuId { get; set; }
    public string MenuName { get; set; }
    public bool IsSelected { get; set; }
}

public class EditMenuGroupViewModel
{
    public string GroupId { get; set; }
    public string GroupName { get; set; }

    // Menus (telas) dentro do grupo
    public List<EditMenuViewModel> Items { get; set; } = [];
}