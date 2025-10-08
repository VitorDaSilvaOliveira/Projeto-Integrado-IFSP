using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Estoque.Domain.Entities;

[Table("MasterData", Schema = "dbo")]
public class MasterData
{
    [Key]
    [Column("name")]
    [StringLength(64)]
    public string Name { get; set; } = null!;

    [Required]
    [Column("type")]
    [StringLength(1)]
    public string Type { get; set; } = null!;

    [Column("tablename")]
    public string? TableName { get; set; }

    [Column("json")]
    public string? Json { get; set; }

    [Column("info")]
    [StringLength(150)]
    public string? Info { get; set; }

    [Column("owner")]
    [StringLength(64)]
    public string? Owner { get; set; }

    [Required]
    [Column("modified")]
    public DateTime Modified { get; set; }

    [Column("sync")]
    public bool? Sync { get; set; }
}
