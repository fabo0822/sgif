using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgif.domain.entities;

namespace sgif.domain.ports
{
    public interface IEmpleadoRepository
    {
        Task<Empleado?> GetByIdAsync(int id);
        Task<IEnumerable<Empleado>> GetAllAsync();
        Task AddAsync(Empleado empleado);
        Task UpdateAsync(Empleado empleado);
        Task DeleteAsync(int id);
    }
}