using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgif.domain.entities;

namespace sgif.domain.ports;

public interface IProductoRepository : IGenericRepository<Producto>
{
    Task<Producto> GetById(string id);
    Task<IEnumerable<Producto>> GetAllAsync();
    Task AddAsync(Producto producto);
    new Task Update(Producto producto);
    Task DeleteAsync(string id);
}