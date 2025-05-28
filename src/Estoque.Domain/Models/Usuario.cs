using System;
using Microsoft.AspNetCore.Identity;

namespace Estoque.Domain.Models
{
    public class Usuario : IdentityUser
    {
        public string Login { get; set; }
        public string Senha { get; set; }
    }
}
