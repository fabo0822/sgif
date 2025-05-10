using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgif.domain.entities;

namespace sgif.domain.ports;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int id);
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);
}