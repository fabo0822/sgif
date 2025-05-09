using System;
using System.Collections.Generic;

namespace sgif.domain.entities
{
    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime FechaPedido { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string Estado { get; set; } = "";
        public decimal Total { get; set; }
        public List<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
