namespace sgif.domain.entities
{
    public class Tercero
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; } // Cliente, Empleado, Proveedor
        public string Documento { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }
} 