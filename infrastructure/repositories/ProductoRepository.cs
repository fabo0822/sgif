using MySql.Data.MySqlClient;
using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.infrastructure.repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Producto?> GetByIdAsync(string id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                "SELECT * FROM Productos WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Producto
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                    StockMin = reader.GetInt32(reader.GetOrdinal("stockMin")),
                    StockMax = reader.GetInt32(reader.GetOrdinal("stockMax")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updatedAt")),
                    Barcode = reader.GetString(reader.GetOrdinal("barcode"))
                };
            }
            return null;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            var productos = new List<Producto>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                "SELECT * FROM Productos", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                    StockMin = reader.GetInt32(reader.GetOrdinal("stockMin")),
                    StockMax = reader.GetInt32(reader.GetOrdinal("stockMax")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updatedAt")),
                    Barcode = reader.GetString(reader.GetOrdinal("barcode"))
                });
            }
            return productos;
        }

        public async Task AddAsync(Producto producto)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                @"INSERT INTO Productos (id, nombre, stock, stockMin, stockMax, createdAt, updatedAt, barcode) 
                VALUES (@id, @nombre, @stock, @stockMin, @stockMax, @createdAt, @updatedAt, @barcode)", conn);

            cmd.Parameters.AddWithValue("@id", producto.Id);
            cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
            cmd.Parameters.AddWithValue("@stock", producto.Stock);
            cmd.Parameters.AddWithValue("@stockMin", producto.StockMin);
            cmd.Parameters.AddWithValue("@stockMax", producto.StockMax);
            cmd.Parameters.AddWithValue("@createdAt", producto.CreatedAt);
            cmd.Parameters.AddWithValue("@updatedAt", producto.UpdatedAt);
            cmd.Parameters.AddWithValue("@barcode", producto.Barcode);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Producto producto)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                @"UPDATE Productos 
                SET nombre = @nombre, stock = @stock, stockMin = @stockMin, stockMax = @stockMax, 
                    updatedAt = @updatedAt, barcode = @barcode 
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@id", producto.Id);
            cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
            cmd.Parameters.AddWithValue("@stock", producto.Stock);
            cmd.Parameters.AddWithValue("@stockMin", producto.StockMin);
            cmd.Parameters.AddWithValue("@stockMax", producto.StockMax);
            cmd.Parameters.AddWithValue("@updatedAt", producto.UpdatedAt);
            cmd.Parameters.AddWithValue("@barcode", producto.Barcode);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(string id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                "DELETE FROM Productos WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
} 