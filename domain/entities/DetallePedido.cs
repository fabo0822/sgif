using System;

namespace sgif.domain.entities
{
    public class DetallePedido
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public string ProductoId { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
