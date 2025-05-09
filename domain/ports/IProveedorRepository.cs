using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgif.domain.entities;

namespace sgif.domain.ports
{
    public interface IProveedorRepository
    {
        Task<Proveedor?> GetByIdAsync(int id);
        Task<IEnumerable<Proveedor>> GetAllAsync();
        Task AddAsync(Proveedor proveedor);
        Task UpdateAsync(Proveedor proveedor);
        Task DeleteAsync(int id);
        Task<IEnumerable<Producto>> GetProductosAsync(int proveedorId);
    }
}