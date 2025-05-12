using sgif.domain.factory;
using sgif.domain.ports;
using sgif.infrastructure.repositories;

namespace sgif.infrastructure.mysql
{
    public class MySqlDbFactory : IDbFactory
    {
        private readonly string _connectionString;

        public MySqlDbFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IClienteRepository CrearClienteRepository()
        {
            return new ClienteRepository(_connectionString);
        }

        public IProductoRepository CrearProductoRepository()
        {
            return new ProductoRepository(_connectionString);
        }

        public IEmpleadoRepository CrearEmpleadoRepository()
        {
            return new EmpleadoRepository(_connectionString);
        }

        public IProveedorRepository CrearProveedorRepository()
        {
            return new ProveedorRepository(_connectionString);
        }

        public ITerceroRepository CrearTerceroRepository()
        {
            return new TerceroRepository(ConexionSingleton.Instancia("your_connection_string_here").ConnectionString);
        }
    }
}