namespace Estoque.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; } // Auto-incremento pelo banco
    // Se você usa Identity, guarde o UserId (GUID, string ou int, dependendo)
    public string? UserId { get; set; }

    public string UserName { get; set; } // Pra mostrar nome direto

    public string? IpAddress { get; set; }

    public DateTime AccessedAt { get; set; }

    public string Area { get; set; } // Ex: Admin

    public string Action { get; set; } // Ex: "Acessou o log de auditoria"

    // Se quiser detalhes extras
    public string Details { get; set; }
}