using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgif.domain.ports
{
    public interface IGenericRepository<T>
    {
        List<T> ObtenerTodos();
        T? ObtenerPorId(int id);
        void Crear(T entity);
        void Actualizar(T entity);
        void Eliminar(int id);
    }
}