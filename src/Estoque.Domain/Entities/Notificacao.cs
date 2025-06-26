namespace Estoque.Domain.Entities;

public class Notificacao
{
    public int Id { get; set; }
    public string Mensagem { get; set; }
    public DateTime Data { get; set; }
    public string IdUser { get; set; }
}