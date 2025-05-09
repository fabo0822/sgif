using MySql.Data.MySqlClient;
using sgif.domain.entities;
using sgif.domain.ports;
using sgif.infrastructure.mysql;

namespace sgif.infrastructure.repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("SELECT * FROM Productos WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Producto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                    Precio = reader.GetDecimal(reader.GetOrdinal("precio"))
                };
            }
            return null;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            var productos = new List<Producto>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("SELECT * FROM Productos", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                    Precio = reader.GetDecimal(reader.GetOrdinal("precio"))
                });
            }
            return productos;
        }

        public async Task AddAsync(Producto producto)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("INSERT INTO Productos (id, nombre, stock, precio) VALUES (@id, @nombre, @stock, @precio)", connection);
            command.Parameters.AddWithValue("@id", producto.Id);
            command.Parameters.AddWithValue("@nombre", producto.Nombre);
            command.Parameters.AddWithValue("@stock", producto.Stock);
            command.Parameters.AddWithValue("@precio", producto.Precio);
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Producto producto)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("UPDATE Productos SET nombre = @nombre, stock = @stock, precio = @precio WHERE id = @id", connection);
            command.Parameters.AddWithValue("@nombre", producto.Nombre);
            command.Parameters.AddWithValue("@stock", producto.Stock);
            command.Parameters.AddWithValue("@precio", producto.Precio);
            command.Parameters.AddWithValue("@id", producto.Id);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("DELETE FROM Productos WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();
        }
    }
} 