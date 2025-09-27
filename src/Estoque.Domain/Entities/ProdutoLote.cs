using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("ProdutoLote")]
public class ProdutoLote
{
    [Key]
    public int LoteId { get; set; }

    [Required]
    public int ProdutoId { get; set; }

    [Required]
    public int FornecedorId { get; set; }

    [Required]
    public int Quantidade { get; set; }

    [Required]
    public int QuantidadeDisponivel { get; set; }

    [Required]
    public decimal CustoUnitario { get; set; }

    public DateTime DataEntrada { get; set; } = DateTime.UtcNow;

    public virtual Produto Produto { get; set; }
    public virtual Fornecedor Fornecedor { get; set; }

    public virtual ICollection<ProdutoSerie> ProdutoSeries { get; set; } = new List<ProdutoSerie>();
}
