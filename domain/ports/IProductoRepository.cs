using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgif.domain.entities;

namespace sgif.domain.ports;

public interface IProductoRepository
{
    Task<Producto?> GetByIdAsync(int id);
    Task<IEnumerable<Producto>> GetAllAsync();
    Task AddAsync(Producto producto);
    Task UpdateAsync(Producto producto);
    Task DeleteAsync(int id);
}