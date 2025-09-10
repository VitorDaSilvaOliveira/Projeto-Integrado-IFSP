using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities
{
    [Table("SolicitacaoDevolucao")]
    public class SolicitacaoDevolucao
    {
        [Key]
        [Column("IdSolicitacao")]
        public int IdSolicitacao { get; set; }

        [Required]
        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [ForeignKey(nameof(IdCliente))]
        public virtual Cliente Cliente { get; set; } = null!;

        [Required]
        [Column("IdProduto")]
        public int IdProduto { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; } = null!;

        [Required]
        [Column("DataSolicitacao")]
        public DateTime DataSolicitacao { get; set; }

        [Required]
        [Column("Quantidade")]
        public int Quantidade { get; set; }

        [Column("Motivo")]
        [StringLength(500)]
        public string? Motivo { get; set; }

        [Required]
        [Column("Devolvido")]
        public bool Devolvido { get; set; } = false;
    }
}
