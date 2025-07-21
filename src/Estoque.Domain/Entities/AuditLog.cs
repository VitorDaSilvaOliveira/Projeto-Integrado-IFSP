namespace Estoque.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public string? UserId { get; set; }

    public string UserName { get; set; } 

    public string? IpAddress { get; set; }

    public DateTime AccessedAt { get; set; }

    public string Area { get; set; }

    public string Action { get; set; } 
    
    public string Details { get; set; }
}