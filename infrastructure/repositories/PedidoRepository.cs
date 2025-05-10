using MySql.Data.MySqlClient;
using sgif.domain.entities;
using sgif.domain.ports;
using sgif.infrastructure.mysql;

namespace sgif.infrastructure.repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly string _connectionString;

        public PedidoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Pedido?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand(@"
                SELECT p.*, d.* 
                FROM Pedidos p 
                LEFT JOIN DetallePedido d ON p.id = d.pedido_id 
                WHERE p.id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            Pedido? pedido = null;
            var detalles = new List<DetallePedido>();

            while (await reader.ReadAsync())
            {
                if (pedido == null)
                {
                    pedido = new Pedido
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        ClienteId = reader.GetInt32(reader.GetOrdinal("cliente_id")),
                        Estado = reader.GetString(reader.GetOrdinal("estado")),
                        Total = reader.GetDecimal(reader.GetOrdinal("total"))
                    };
                }

                if (!reader.IsDBNull(reader.GetOrdinal("detalle_id")))
                {
                    detalles.Add(new DetallePedido
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("detalle_id")),
                        ProductoId = reader.GetInt32(reader.GetOrdinal("producto_id")).ToString(),
                        Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                        PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("precio_unitario")),
                        Subtotal = reader.GetDecimal(reader.GetOrdinal("subtotal"))
                    });
                }
            }

            if (pedido != null)
            {
                pedido.Detalles = detalles;
            }
            return pedido;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            var pedidos = new List<Pedido>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("SELECT * FROM Pedidos", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                pedidos.Add(new Pedido
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    ClienteId = reader.GetInt32(reader.GetOrdinal("cliente_id")),
                    Estado = reader.GetString(reader.GetOrdinal("estado")),
                    Total = reader.GetDecimal(reader.GetOrdinal("total"))
                });
            }
            return pedidos;
        }

        public async Task AddAsync(Pedido pedido)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Insertar pedido
                var commandPedido = new MySqlCommand(@"
                    INSERT INTO Pedidos (cliente_id, estado, total) 
                    VALUES (@cliente_id, @estado, @total);
                    SELECT LAST_INSERT_ID();", connection, transaction);
                
                commandPedido.Parameters.AddWithValue("@cliente_id", pedido.ClienteId);
                commandPedido.Parameters.AddWithValue("@estado", pedido.Estado);
                commandPedido.Parameters.AddWithValue("@total", pedido.Total);

                var pedidoId = Convert.ToInt32(await commandPedido.ExecuteScalarAsync());

                // Insertar detalles
                foreach (var detalle in pedido.Detalles)
                {
                    var commandDetalle = new MySqlCommand(@"
                        INSERT INTO DetallePedido (pedido_id, producto_id, cantidad, precio_unitario, subtotal) 
                        VALUES (@pedido_id, @producto_id, @cantidad, @precio_unitario, @subtotal)", 
                        connection, transaction);

                    commandDetalle.Parameters.AddWithValue("@pedido_id", pedidoId);
                    commandDetalle.Parameters.AddWithValue("@producto_id", detalle.ProductoId);
                    commandDetalle.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
                    commandDetalle.Parameters.AddWithValue("@precio_unitario", detalle.PrecioUnitario);
                    commandDetalle.Parameters.AddWithValue("@subtotal", detalle.Subtotal);

                    await commandDetalle.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Pedido pedido)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand(@"
                UPDATE Pedidos 
                SET estado = @estado, total = @total 
                WHERE id = @id", connection);
            
            command.Parameters.AddWithValue("@estado", pedido.Estado);
            command.Parameters.AddWithValue("@total", pedido.Total);
            command.Parameters.AddWithValue("@id", pedido.Id);
            
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Eliminar detalles primero
                var commandDetalles = new MySqlCommand("DELETE FROM DetallePedido WHERE pedido_id = @id", connection, transaction);
                commandDetalles.Parameters.AddWithValue("@id", id);
                await commandDetalles.ExecuteNonQueryAsync();

                // Eliminar pedido
                var commandPedido = new MySqlCommand("DELETE FROM Pedidos WHERE id = @id", connection, transaction);
                commandPedido.Parameters.AddWithValue("@id", id);
                await commandPedido.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
} 