using System;

namespace sgif.domain.entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string TerceroId { get; set; } = "";
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaCompra { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string Email { get; set; } = "";
        public int TipoDocumentoId { get; set; }
        public int TipoTerceroId { get; set; }
        public int CiudadId { get; set; }
    }
}
