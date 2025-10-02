using System.ComponentModel.DataAnnotations.Schema;
using Estoque.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Estoque.Domain.Entities;

[Table("Pedido")]
public class Pedido
{
    // --- SUAS PROPRIEDADES ANTIGAS (INTACTAS) ---
    public int Id { get; set; }
    public string? NumeroPedido { get; set; }
    public DateTime? DataPedido { get; set; }
    [Column("Cliente_Id")]
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public decimal? ValorTotal { get; set; }
    public PedidoStatus? Status { get; set; }
    public int? Operacao { get; set; }
    public DateTime? DataEntrega { get; set; }
    public string? Observacoes { get; set; }
    public string? UsuarioResponsavel { get; set; }

    // --- NOVA PROPRIEDADE ESSENCIAL ---
    public virtual ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();

    // --- NOVO MÉTODO (CORRIGIDO) ---
    public void AdicionarItem(PedidoItem novoItem)
    {
        // Regra de Negócio 1: Não permitir adicionar produto sem estoque
        // A CORREÇÃO ESTÁ AQUI: GetValueOrDefault(0)
        if (novoItem.Produto == null || novoItem.Produto.QuantidadeEstoque.GetValueOrDefault(0) <= 0)
        {
            throw new InvalidOperationException("Não é possível adicionar um produto sem estoque.");
        }

        // Regra de Negócio 2: Se o item já existe, apenas somar a quantidade
        var itemExistente = Itens.FirstOrDefault(i => i.ProdutoId == novoItem.ProdutoId);

        if (itemExistente != null)
        {
            itemExistente.Quantidade += novoItem.Quantidade;
        }
        else
        {
            Itens.Add(novoItem);
        }
    }
}