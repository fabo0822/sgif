using MySql.Data.MySqlClient;
using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.infrastructure.repositories
{
    public class DetallePedidoRepository : IDetallePedidoRepository
    {
        private readonly string _connectionString;

        public DetallePedidoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<DetallePedido?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("SELECT * FROM DetallePedido WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DetallePedido
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    PedidoId = reader.GetInt32(reader.GetOrdinal("pedido_id")),
                    ProductoId = reader.GetString(reader.GetOrdinal("producto_id")),
                    Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                    PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("precio_unitario")),
                    Subtotal = reader.GetDecimal(reader.GetOrdinal("subtotal"))
                };
            }
            return null;
        }

        public async Task<IEnumerable<DetallePedido>> GetAllAsync()
        {
            var detalles = new List<DetallePedido>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("SELECT * FROM DetallePedido", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                detalles.Add(new DetallePedido
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    PedidoId = reader.GetInt32(reader.GetOrdinal("pedido_id")),
                    ProductoId = reader.GetString(reader.GetOrdinal("producto_id")),
                    Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                    PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("precio_unitario")),
                    Subtotal = reader.GetDecimal(reader.GetOrdinal("subtotal"))
                });
            }
            return detalles;
        }

        public async Task<IEnumerable<DetallePedido>> GetByPedidoIdAsync(int pedidoId)
        {
            var detalles = new List<DetallePedido>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("SELECT * FROM DetallePedido WHERE pedido_id = @pedidoId", connection);
            command.Parameters.AddWithValue("@pedidoId", pedidoId);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                detalles.Add(new DetallePedido
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    PedidoId = reader.GetInt32(reader.GetOrdinal("pedido_id")),
                    ProductoId = reader.GetString(reader.GetOrdinal("producto_id")),
                    Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                    PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("precio_unitario")),
                    Subtotal = reader.GetDecimal(reader.GetOrdinal("subtotal"))
                });
            }
            return detalles;
        }

        public async Task AddAsync(DetallePedido detalle)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand(@"
                INSERT INTO DetallePedido (pedido_id, producto_id, cantidad, precio_unitario, subtotal) 
                VALUES (@pedidoId, @productoId, @cantidad, @precioUnitario, @subtotal)", connection);
            
            command.Parameters.AddWithValue("@pedidoId", detalle.PedidoId);
            command.Parameters.AddWithValue("@productoId", detalle.ProductoId);
            command.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
            command.Parameters.AddWithValue("@precioUnitario", detalle.PrecioUnitario);
            command.Parameters.AddWithValue("@subtotal", detalle.Subtotal);
            
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(DetallePedido detalle)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand(@"
                UPDATE DetallePedido 
                SET pedido_id = @pedidoId, 
                    producto_id = @productoId, 
                    cantidad = @cantidad, 
                    precio_unitario = @precioUnitario, 
                    subtotal = @subtotal 
                WHERE id = @id", connection);
            
            command.Parameters.AddWithValue("@id", detalle.Id);
            command.Parameters.AddWithValue("@pedidoId", detalle.PedidoId);
            command.Parameters.AddWithValue("@productoId", detalle.ProductoId);
            command.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
            command.Parameters.AddWithValue("@precioUnitario", detalle.PrecioUnitario);
            command.Parameters.AddWithValue("@subtotal", detalle.Subtotal);
            
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("DELETE FROM DetallePedido WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();
        }
    }
} 