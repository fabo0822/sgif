namespace sgif.domain.entities
{
    public class Tercero
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TipoDocumentoId { get; set; }
        public int TipoTerceroId { get; set; }
        public int CiudadId { get; set; }
    }
} 