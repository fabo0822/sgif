using sgif.domain.entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sgif.domain.ports
{
    public interface IVentaRepository : IGenericRepository<Venta>
    {
        Task<List<Venta>> GetVentasByCliente(string clienteId);
        Task<List<Venta>> GetVentasByEmpleado(string empleadoId);
        Task<List<DetalleVenta>> GetDetallesVenta(int ventaId);
    }
} 