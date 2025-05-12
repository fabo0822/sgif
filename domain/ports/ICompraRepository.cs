using sgif.domain.entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sgif.domain.ports
{
    public interface ICompraRepository : IGenericRepository<Compra>
    {
        Task<List<Compra>> GetComprasByProveedor(string proveedorId);
        Task<List<Compra>> GetComprasByEmpleado(string empleadoId);
        Task<List<DetalleCompra>> GetDetallesCompra(int compraId);
    }
} 