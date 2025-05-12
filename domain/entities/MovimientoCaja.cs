using System;

namespace sgif.domain.entities
{
    public class MovimientoCaja
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int TipoMovimientoId { get; set; }
        public decimal Valor { get; set; }
        public string? Concepto { get; set; }
        public string? TerceroId { get; set; }
        public TipoMovimientoCaja? TipoMovimiento { get; set; }
    }
}