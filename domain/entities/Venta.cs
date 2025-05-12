using System;
using System.Collections.Generic;

namespace sgif.domain.entities
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string TerceroEmpId { get; set; }
        public int ClienteId { get; set; }
        public int? FactId { get; set; }
        public List<DetalleVenta> Detalles { get; set; }
    }
} 