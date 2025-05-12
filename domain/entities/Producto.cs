using System;

namespace sgif.domain.entities
{
    public class Producto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int StockMin { get; set; }
        public int StockMax { get; set; }
        public decimal PrecioVenta { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Barcode { get; set; } = string.Empty;
    }
}
