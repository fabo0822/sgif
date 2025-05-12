using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgif.domain.ports
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T> GetById(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
    }
}