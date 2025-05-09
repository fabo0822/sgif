using System;
public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int EmpleadoId { get; set; }
    public DateTime FechaPedido { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public string Estado { get; set; } = "";
}
