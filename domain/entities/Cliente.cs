using System;

namespace sgif.domain.entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Direccion { get; set; } = "";
        public string Ciudad { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Correo { get; set; } = "";
    }
}
