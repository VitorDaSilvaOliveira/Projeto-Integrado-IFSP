using Estoque.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Devolucao")]
public class Devolucao
{
    [Key]
    [Column("idDevolucao")]
    public int IdDevolucao { get; set; }

    [Column("TipoDevolucao")]
    public byte TipoDevolucao { get; set; } // 1=Cliente->Loja, 2=Loja->Fornecedor

    [Column("IdPedido")]
    public int? IdPedido { get; set; }

    [Column("IdFornecedor")]
    public int? IdFornecedor { get; set; }

    [Column("DataDevolucao", TypeName = "date")]
    public DateTime DataDevolucao { get; set; }

    [Column("Observacao")]
    [StringLength(255)]
    public string? Observacao { get; set; }

    [Column("Id_User")]
    [StringLength(100)]
    public string IdUser { get; set; }

    // 🔗 Relacionamentos
    public virtual ICollection<DevolucaoItem> Itens { get; set; }
}
