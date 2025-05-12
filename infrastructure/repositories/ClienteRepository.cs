using MySql.Data.MySqlClient;
using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.infrastructure.repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string _connectionString;

        public ClienteRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand(@"
                SELECT c.*, t.nombre, t.apellidos, t.email, t.tipo_doc_id, t.tipo_tercero_id, t.ciudad_id 
                FROM Cliente c 
                JOIN Terceros t ON c.tercero_id = t.id 
                WHERE c.id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Cliente
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroId = reader.GetString(reader.GetOrdinal("tercero_id")),
                    FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("fecha_nac")),
                    FechaCompra = reader.GetDateTime(reader.GetOrdinal("fecha_compra")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Apellidos = reader.GetString(reader.GetOrdinal("apellidos")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    TipoDocumentoId = reader.GetInt32(reader.GetOrdinal("tipo_doc_id")),
                    TipoTerceroId = reader.GetInt32(reader.GetOrdinal("tipo_tercero_id")),
                    CiudadId = reader.GetInt32(reader.GetOrdinal("ciudad_id"))
                };
            }
            return null;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            var clientes = new List<Cliente>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand(@"
                SELECT c.*, t.nombre, t.apellidos, t.email, t.tipo_doc_id, t.tipo_tercero_id, t.ciudad_id 
                FROM Cliente c 
                JOIN Terceros t ON c.tercero_id = t.id", connection);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                clientes.Add(new Cliente
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroId = reader.GetString(reader.GetOrdinal("tercero_id")),
                    FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("fecha_nac")),
                    FechaCompra = reader.GetDateTime(reader.GetOrdinal("fecha_compra")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Apellidos = reader.GetString(reader.GetOrdinal("apellidos")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    TipoDocumentoId = reader.GetInt32(reader.GetOrdinal("tipo_doc_id")),
                    TipoTerceroId = reader.GetInt32(reader.GetOrdinal("tipo_tercero_id")),
                    CiudadId = reader.GetInt32(reader.GetOrdinal("ciudad_id"))
                });
            }
            return clientes;
        }

        public async Task AddAsync(Cliente cliente)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var command = new MySqlCommand("CALL insertar_tercero_y_cliente(@id, @nombre, @apellidos, @email, @tipo_doc_id, @tipo_tercero_id, @ciudad_id, @fecha_nac, @fecha_ultima_compra)", connection, transaction);

                command.Parameters.AddWithValue("@id", cliente.TerceroId);
                command.Parameters.AddWithValue("@nombre", cliente.Nombre);
                command.Parameters.AddWithValue("@apellidos", cliente.Apellidos);
                command.Parameters.AddWithValue("@email", cliente.Email);
                command.Parameters.AddWithValue("@tipo_doc_id", cliente.TipoDocumentoId);
                command.Parameters.AddWithValue("@tipo_tercero_id", cliente.TipoTerceroId);
                command.Parameters.AddWithValue("@ciudad_id", cliente.CiudadId);
                command.Parameters.AddWithValue("@fecha_nac", cliente.FechaNacimiento);
                command.Parameters.AddWithValue("@fecha_ultima_compra", cliente.FechaCompra);

                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Actualizamos Terceros
                var terceroCommand = new MySqlCommand(@"
                    UPDATE Terceros 
                    SET nombre = @nombre, 
                        apellidos = @apellidos, 
                        email = @email, 
                        tipo_doc_id = @tipo_doc_id, 
                        tipo_tercero_id = @tipo_tercero_id, 
                        ciudad_id = @ciudad_id 
                    WHERE id = @id", 
                    connection, transaction);

                terceroCommand.Parameters.AddWithValue("@id", cliente.TerceroId);
                terceroCommand.Parameters.AddWithValue("@nombre", cliente.Nombre);
                terceroCommand.Parameters.AddWithValue("@apellidos", cliente.Apellidos);
                terceroCommand.Parameters.AddWithValue("@email", cliente.Email);
                terceroCommand.Parameters.AddWithValue("@tipo_doc_id", cliente.TipoDocumentoId);
                terceroCommand.Parameters.AddWithValue("@tipo_tercero_id", cliente.TipoTerceroId);
                terceroCommand.Parameters.AddWithValue("@ciudad_id", cliente.CiudadId);
                await terceroCommand.ExecuteNonQueryAsync();

                // Actualizamos Cliente
                var clienteCommand = new MySqlCommand(@"
                    UPDATE Cliente 
                    SET fecha_nac = @fecha_nac, 
                        fecha_compra = @fecha_compra 
                    WHERE id = @id", 
                    connection, transaction);

                clienteCommand.Parameters.AddWithValue("@id", cliente.Id);
                clienteCommand.Parameters.AddWithValue("@fecha_nac", cliente.FechaNacimiento);
                clienteCommand.Parameters.AddWithValue("@fecha_compra", cliente.FechaCompra);
                await clienteCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Primero obtenemos el tercero_id
                var getTerceroCommand = new MySqlCommand("SELECT tercero_id FROM Cliente WHERE id = @id", connection, transaction);
                getTerceroCommand.Parameters.AddWithValue("@id", id);
                var terceroId = await getTerceroCommand.ExecuteScalarAsync() as string;

                if (terceroId != null)
                {
                    // Eliminamos de Cliente
                    var clienteCommand = new MySqlCommand("DELETE FROM Cliente WHERE id = @id", connection, transaction);
                    clienteCommand.Parameters.AddWithValue("@id", id);
                    await clienteCommand.ExecuteNonQueryAsync();

                    // Eliminamos de Terceros
                    var terceroCommand = new MySqlCommand("DELETE FROM Terceros WHERE id = @id", connection, transaction);
                    terceroCommand.Parameters.AddWithValue("@id", terceroId);
                    await terceroCommand.ExecuteNonQueryAsync();
                }

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