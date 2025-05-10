using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgif.domain.entities;

namespace sgif.domain.ports;

public interface IPedidoRepository
{
    Task<Pedido?> GetByIdAsync(int id);
    Task<IEnumerable<Pedido>> GetAllAsync();
    Task AddAsync(Pedido pedido);
    Task UpdateAsync(Pedido pedido);
    Task DeleteAsync(int id);
}