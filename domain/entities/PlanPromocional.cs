using System;

namespace sgif.domain.entities
{
    public class PlanPromocional
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public double Descuento { get; set; }
        public List<PlanProducto> Productos { get; set; }
    }
} 