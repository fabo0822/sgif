using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgif.domain.ports;

namespace sgif.domain.factory
{
    public interface IDbFactory
    {
        IClienteRepository CrearClienteRepository();
        IProductoRepository CrearProductoRepository();
        IEmpleadoRepository CrearEmpleadoRepository();
        IProveedorRepository CrearProveedorRepository();
        ITerceroRepository CrearTerceroRepository(); // Nuevo método agregado
    }
}