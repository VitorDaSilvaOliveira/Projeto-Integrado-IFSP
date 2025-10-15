using System.ComponentModel.DataAnnotations.Schema;

namespace Estoque.Domain.Entities;

[Table("PedidoItem")]
public class PedidoItem
{
    public int Id { get; set; }
    [Column("id_Pedido")]
    public int PedidoId { get; set; }
    public Pedido Pedido { get; set; }
    [Column("id_Produto")]
    public int ProdutoId { get; set; }
    public Produto Produto { get; set; }
    public int Quantidade { get; set; }
    public decimal Desconto { get; set; }
    public decimal PrecoTabela { get; set; }
    public decimal PrecoVenda { get; set; }

    public decimal ValorTotal { get; set; }

}