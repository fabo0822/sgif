using System;
public class Envio
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public string DireccionEnvio { get; set; } = "";
    public string Ciudad { get; set; } = "";
    public string MetodoEnvio { get; set; } = "";
}
