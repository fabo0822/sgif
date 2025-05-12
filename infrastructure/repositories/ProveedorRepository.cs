using MySql.Data.MySqlClient;
using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.infrastructure.repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly string _connectionString;

        public ProveedorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Proveedor?> GetByIdAsync(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                @"SELECT p.*, t.nombre, t.apellidos, t.email, t.tipo_doc_id, t.tipo_tercero_id, t.ciudad_id 
                FROM Proveedor p 
                JOIN Terceros t ON p.tercero_id = t.id 
                WHERE p.id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Proveedor
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroId = reader.GetString(reader.GetOrdinal("tercero_id")),
                    FechaIngreso = reader.GetDateTime(reader.GetOrdinal("fecha_ingreso")),
                    Descuento = reader.GetDouble(reader.GetOrdinal("descuento")),
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

        public async Task<IEnumerable<Proveedor>> GetAllAsync()
        {
            var proveedores = new List<Proveedor>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                @"SELECT p.*, t.nombre, t.apellidos, t.email, t.tipo_doc_id, t.tipo_tercero_id, t.ciudad_id 
                FROM Proveedor p 
                JOIN Terceros t ON p.tercero_id = t.id", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                proveedores.Add(new Proveedor
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroId = reader.GetString(reader.GetOrdinal("tercero_id")),
                    FechaIngreso = reader.GetDateTime(reader.GetOrdinal("fecha_ingreso")),
                    Descuento = reader.GetDouble(reader.GetOrdinal("descuento")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Apellidos = reader.GetString(reader.GetOrdinal("apellidos")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    TipoDocumentoId = reader.GetInt32(reader.GetOrdinal("tipo_doc_id")),
                    TipoTerceroId = reader.GetInt32(reader.GetOrdinal("tipo_tercero_id")),
                    CiudadId = reader.GetInt32(reader.GetOrdinal("ciudad_id"))
                });
            }
            return proveedores;
        }

        public async Task AddAsync(Proveedor proveedor)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                var command = new MySqlCommand("CALL insertar_tercero_y_proveedor(@id, @nombre, @apellidos, @email, @tipo_doc_id, @tipo_tercero_id, @ciudad_id, @fecha_ingreso, @descuento)", conn, transaction);

                var terceroId = Guid.NewGuid().ToString("N").Substring(0, 20);
                command.Parameters.AddWithValue("@id", terceroId);
                command.Parameters.AddWithValue("@nombre", proveedor.Nombre);
                command.Parameters.AddWithValue("@apellidos", proveedor.Apellidos);
                command.Parameters.AddWithValue("@email", proveedor.Email);
                command.Parameters.AddWithValue("@tipo_doc_id", proveedor.TipoDocumentoId);
                command.Parameters.AddWithValue("@tipo_tercero_id", proveedor.TipoTerceroId);
                command.Parameters.AddWithValue("@ciudad_id", proveedor.CiudadId);
                command.Parameters.AddWithValue("@fecha_ingreso", proveedor.FechaIngreso.Date);
                command.Parameters.AddWithValue("@descuento", proveedor.Descuento);

                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al registrar proveedor: {ex.Message}\nStackTrace: {ex.StackTrace}", ex);
            }
        }

        public async Task UpdateAsync(Proveedor proveedor)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                // Actualizar Terceros
                var cmdTercero = new MySqlCommand(
                    @"UPDATE Terceros 
                    SET nombre = @nombre, apellidos = @apellidos, email = @email, 
                        tipo_doc_id = @tipo_doc_id, ciudad_id = @ciudad_id 
                    WHERE id = @id", conn, transaction);

                cmdTercero.Parameters.AddWithValue("@id", proveedor.TerceroId);
                cmdTercero.Parameters.AddWithValue("@nombre", proveedor.Nombre);
                cmdTercero.Parameters.AddWithValue("@apellidos", proveedor.Apellidos);
                cmdTercero.Parameters.AddWithValue("@email", proveedor.Email);
                cmdTercero.Parameters.AddWithValue("@tipo_doc_id", proveedor.TipoDocumentoId);
                cmdTercero.Parameters.AddWithValue("@ciudad_id", proveedor.CiudadId);

                await cmdTercero.ExecuteNonQueryAsync();

                // Actualizar Proveedor
                var cmdProveedor = new MySqlCommand(
                    @"UPDATE Proveedor 
                    SET fecha_ingreso = @fecha_ingreso, descuento = @descuento 
                    WHERE tercero_id = @tercero_id", conn, transaction);

                cmdProveedor.Parameters.AddWithValue("@tercero_id", proveedor.TerceroId);
                cmdProveedor.Parameters.AddWithValue("@fecha_ingreso", proveedor.FechaIngreso);
                cmdProveedor.Parameters.AddWithValue("@descuento", proveedor.Descuento);

                await cmdProveedor.ExecuteNonQueryAsync();
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
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                // Obtener el tercero_id antes de eliminar
                var cmdGetTercero = new MySqlCommand(
                    "SELECT tercero_id FROM Proveedor WHERE id = @id", conn, transaction);
                cmdGetTercero.Parameters.AddWithValue("@id", id);

                var terceroId = (string?)await cmdGetTercero.ExecuteScalarAsync();
                if (terceroId == null) return;

                // Eliminar Proveedor
                var cmdProveedor = new MySqlCommand(
                    "DELETE FROM Proveedor WHERE id = @id", conn, transaction);
                cmdProveedor.Parameters.AddWithValue("@id", id);
                await cmdProveedor.ExecuteNonQueryAsync();

                // Eliminar Tercero
                var cmdTercero = new MySqlCommand(
                    "DELETE FROM Terceros WHERE id = @id", conn, transaction);
                cmdTercero.Parameters.AddWithValue("@id", terceroId);
                await cmdTercero.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Producto>> GetProductosAsync(int proveedorId)
        {
            var productos = new List<Producto>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            // Primero obtener el tercero_id del proveedor
            var cmdGetTercero = new MySqlCommand(
                "SELECT tercero_id FROM Proveedor WHERE id = @id", conn);
            cmdGetTercero.Parameters.AddWithValue("@id", proveedorId);
            var terceroId = (string?)await cmdGetTercero.ExecuteScalarAsync();
            
            if (terceroId == null) return productos;

            var cmd = new MySqlCommand(
                @"SELECT p.* 
                FROM Productos p 
                JOIN Producto_Proveedor pp ON p.id = pp.producto_id 
                WHERE pp.tercero_id = @tercero_id", conn);
            cmd.Parameters.AddWithValue("@tercero_id", terceroId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                    StockMin = reader.GetInt32(reader.GetOrdinal("stockMin")),
                    StockMax = reader.GetInt32(reader.GetOrdinal("stockMax"))
                });
            }
            return productos;
        }
    }
} 