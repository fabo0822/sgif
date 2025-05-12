using sgif.domain.entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace sgif.domain.ports
{
    public interface ITerceroRepository : IGenericRepository<Tercero>
    {
        Task<Tercero> GetById(string id);
        Task<IEnumerable<Tercero>> GetAllAsync();
    }
}