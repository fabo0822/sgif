using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgif.domain.entities;

namespace sgif.domain.ports
{
    public interface IDetallePedidoRepository
    {
        Task<DetallePedido?> GetByIdAsync(int id);
        Task<IEnumerable<DetallePedido>> GetAllAsync();
        Task<IEnumerable<DetallePedido>> GetByPedidoIdAsync(int pedidoId);
        Task AddAsync(DetallePedido detalle);
        Task UpdateAsync(DetallePedido detalle);
        Task DeleteAsync(int id);
    }
}