using System;

namespace sgif.domain.entities
{
    public class Compra
    {
        public int Id { get; set; }
        public string TerceroProveedorId { get; set; }
        public DateTime Fecha { get; set; }
        public string TerceroEmpleadoId { get; set; }
        public string DocCompra { get; set; }
        public List<DetalleCompra> Detalles { get; set; }
    }
} 