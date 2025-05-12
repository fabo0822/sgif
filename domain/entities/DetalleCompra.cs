using System;

namespace sgif.domain.entities
{
    public class DetalleCompra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Valor { get; set; }
        public int CompraId { get; set; }
        public string EntradaSalida { get; set; }
        public Producto Producto { get; set; }
    }
} 