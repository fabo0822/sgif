namespace sgif.domain.entities
{
    public class PlanProducto
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public string ProductoId { get; set; }
        public PlanPromocional Plan { get; set; }
        public Producto Producto { get; set; }
    }
} 