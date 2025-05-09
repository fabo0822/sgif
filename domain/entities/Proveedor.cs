using System;
namespace sgif.domain.entities
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string TerceroId { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
        public double Descuento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TipoDocumentoId { get; set; }
        public int TipoTerceroId { get; set; }
        public int CiudadId { get; set; }
    }
}
