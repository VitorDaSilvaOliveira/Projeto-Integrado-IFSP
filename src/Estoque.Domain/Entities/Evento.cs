using System;

namespace Estoque.Domain.Entities
{
    public class Evento
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public DateTime Data { get; set; }
        public bool Processado { get; set; }

        // 👇 Novos campos
        public int? IdRegistro { get; set; }
        public string Campo { get; set; }
        public string ValorAntigo { get; set; }
        public string ValorNovo { get; set; }
    }
}
