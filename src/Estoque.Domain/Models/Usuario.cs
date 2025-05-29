using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Models
{
    [Table("Usuario")]
    public class Usuario : IdentityUser
    {
        [Key]
        [Column("id_usuario")]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("nome")]
        public string Nome { get; set; }

        [Required]
        [Column("email")]
        public override string Email { get; set; }

        [Required]
        [Column("senha_hash")]
        public string SenhaHash { get; set; }

        [Required]
        [Column("tipo_usuario")]
        public string TipoUsuario { get; set; }
    }
}
