﻿using Estoque.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Estoque.Domain.Models;

public class RolesIndexViewModel
{
    public List<RoleWithUsersViewModel> RolesWithUsers { get; set; } = new();
}

public class RoleWithUsersViewModel
{
    public ApplicationRole Role { get; set; }
    public List<ApplicationUser> Users { get; set; } = new();
}