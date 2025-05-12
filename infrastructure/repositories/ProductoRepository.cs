using MySql.Data.MySqlClient;
using sgif.domain.entities;
using sgif.domain.ports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace sgif.infrastructure.repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Producto>> GetAll()
        {
            var productos = await GetAllAsync();
            return productos.ToList();
        }

        public async Task<Producto> GetById(int id)
        {
            return await GetById(id.ToString());
        }

        public async Task Add(Producto producto)
        {
            await AddAsync(producto);
        }

        public async Task Delete(int id)
        {
            await DeleteAsync(id.ToString());
        }

        public async Task<Producto> GetById(string id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("SELECT * FROM Producto WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Producto
                {
                    Id = reader["id"]?.ToString() ?? string.Empty,
                    Nombre = reader["nombre"]?.ToString() ?? string.Empty,
                    Stock = Convert.ToInt32(reader["stock"]),
                    StockMin = Convert.ToInt32(reader["stockMin"]),
                    StockMax = Convert.ToInt32(reader["stockMax"]),
                    CreatedAt = Convert.ToDateTime(reader["createdAt"]),
                    UpdatedAt = Convert.ToDateTime(reader["updatedAt"]),
                    Barcode = reader["barcode"]?.ToString() ?? string.Empty
                };
            }
            return new Producto();
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            var productos = new List<Producto>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("SELECT * FROM Producto", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                productos.Add(new Producto
                {
                    Id = reader["id"]?.ToString() ?? string.Empty,
                    Nombre = reader["nombre"]?.ToString() ?? string.Empty,
                    Stock = Convert.ToInt32(reader["stock"]),
                    StockMin = Convert.ToInt32(reader["stockMin"]),
                    StockMax = Convert.ToInt32(reader["stockMax"]),
                    CreatedAt = Convert.ToDateTime(reader["createdAt"]),
                    UpdatedAt = Convert.ToDateTime(reader["updatedAt"]),
                    Barcode = reader["barcode"]?.ToString() ?? string.Empty
                });
            }
            return productos;
        }

        public async Task AddAsync(Producto producto)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand(@"
                INSERT INTO Producto (id, nombre, stock, stockMin, stockMax, createdAt, updatedAt, barcode)
                VALUES (@id, @nombre, @stock, @stockMin, @stockMax, @createdAt, @updatedAt, @barcode)", connection);
            
            command.Parameters.AddWithValue("@id", producto.Id);
            command.Parameters.AddWithValue("@nombre", producto.Nombre);
            command.Parameters.AddWithValue("@stock", producto.Stock);
            command.Parameters.AddWithValue("@stockMin", producto.StockMin);
            command.Parameters.AddWithValue("@stockMax", producto.StockMax);
            command.Parameters.AddWithValue("@createdAt", DateTime.Now);
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
            command.Parameters.AddWithValue("@barcode", producto.Barcode);
            
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(Producto producto)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand(@"
                UPDATE Producto 
                SET nombre = @nombre,
                    stock = @stock,
                    stockMin = @stockMin,
                    stockMax = @stockMax,
                    updatedAt = @updatedAt,
                    barcode = @barcode
                WHERE id = @id", connection);
            
            command.Parameters.AddWithValue("@id", producto.Id);
            command.Parameters.AddWithValue("@nombre", producto.Nombre);
            command.Parameters.AddWithValue("@stock", producto.Stock);
            command.Parameters.AddWithValue("@stockMin", producto.StockMin);
            command.Parameters.AddWithValue("@stockMax", producto.StockMax);
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
            command.Parameters.AddWithValue("@barcode", producto.Barcode);
            
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(string id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("DELETE FROM Producto WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            
            await command.ExecuteNonQueryAsync();
        }
    }
}