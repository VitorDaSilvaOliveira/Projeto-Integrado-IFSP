using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("ProdutoFornecedor")]
public class ProdutoFornecedor
{
    public int IdProduto { get; set; }
    public int IdFornecedor { get; set; }

    public decimal PrecoFornecedor { get; set; }
    public int LeadTimeDias { get; set; }

    public virtual Produto Produto { get; set; }
    public virtual Fornecedor Fornecedor { get; set; }
}