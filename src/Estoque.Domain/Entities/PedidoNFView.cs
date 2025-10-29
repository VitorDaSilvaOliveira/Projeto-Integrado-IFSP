namespace Estoque.Domain.Entities;

public class PedidoNFView
{
    public int PedidoId { get; set; }
    public string NumeroPedido { get; set; }
    public DateTime DataPedido { get; set; }
    public decimal ValorNF { get; set; }
    public int FormaPagamento { get; set; }
    public string ClienteCNPJ { get; set; }
    public string ClienteNome { get; set; }
    public int id_Produto { get; set; }
    public string ProdutoNome { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal ItemTotal { get; set; }
}
