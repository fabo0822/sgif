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
                LEFT JOIN Terceros t ON p.tercero_id = t.id 
                WHERE p.id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Proveedor
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroId = reader.IsDBNull(reader.GetOrdinal("tercero_id")) ? string.Empty : reader.GetString(reader.GetOrdinal("tercero_id")),
                    FechaIngreso = reader.IsDBNull(reader.GetOrdinal("fecha_ingreso")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("fecha_ingreso")),
                    Descuento = reader.IsDBNull(reader.GetOrdinal("descuento")) ? 0 : reader.GetDouble(reader.GetOrdinal("descuento")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombre")),
                    Apellidos = reader.IsDBNull(reader.GetOrdinal("apellidos")) ? string.Empty : reader.GetString(reader.GetOrdinal("apellidos")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                    TipoDocumentoId = reader.IsDBNull(reader.GetOrdinal("tipo_doc_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("tipo_doc_id")),
                    TipoTerceroId = reader.IsDBNull(reader.GetOrdinal("tipo_tercero_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("tipo_tercero_id")),
                    CiudadId = reader.IsDBNull(reader.GetOrdinal("ciudad_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("ciudad_id"))
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
                LEFT JOIN Terceros t ON p.tercero_id = t.id", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                proveedores.Add(new Proveedor
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroId = reader.IsDBNull(reader.GetOrdinal("tercero_id")) ? string.Empty : reader.GetString(reader.GetOrdinal("tercero_id")),
                    FechaIngreso = reader.IsDBNull(reader.GetOrdinal("fecha_ingreso")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("fecha_ingreso")),
                    Descuento = reader.IsDBNull(reader.GetOrdinal("descuento")) ? 0 : reader.GetDouble(reader.GetOrdinal("descuento")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombre")),
                    Apellidos = reader.IsDBNull(reader.GetOrdinal("apellidos")) ? string.Empty : reader.GetString(reader.GetOrdinal("apellidos")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                    TipoDocumentoId = reader.IsDBNull(reader.GetOrdinal("tipo_doc_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("tipo_doc_id")),
                    TipoTerceroId = reader.IsDBNull(reader.GetOrdinal("tipo_tercero_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("tipo_tercero_id")),
                    CiudadId = reader.IsDBNull(reader.GetOrdinal("ciudad_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("ciudad_id"))
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
                // Verificar la estructura de la tabla Proveedor
                var cmdCheckStructure = new MySqlCommand(
                    "SHOW COLUMNS FROM Proveedor", conn, transaction);
                using var reader = await cmdCheckStructure.ExecuteReaderAsync();
                var columns = new List<string>();
                while (await reader.ReadAsync())
                {
                    columns.Add(reader.GetString(0));
                }
                await reader.CloseAsync();

                if (!columns.Contains("fecha_ingreso"))
                {
                    throw new Exception($"La tabla Proveedor no tiene la columna fecha_ingreso. Columnas encontradas: {string.Join(", ", columns)}");
                }

                // Verificar si existe el tipo de tercero seleccionado
                var cmdCheckTipo = new MySqlCommand(
                    "SELECT COUNT(*) FROM TipoTercero WHERE id = @tipo_tercero_id", conn, transaction);
                cmdCheckTipo.Parameters.AddWithValue("@tipo_tercero_id", proveedor.TipoTerceroId);
                var tipoExists = Convert.ToInt32(await cmdCheckTipo.ExecuteScalarAsync()) > 0;

                if (!tipoExists)
                {
                    throw new Exception($"El tipo de tercero con ID {proveedor.TipoTerceroId} no existe. Los tipos válidos son: 3 (Proveedor), 6 (Proveedor Premium), 7 (Proveedor Local), 12 (Proveedor Internacional), 13 (Proveedor Nacional), 18 (Proveedor Exclusivo)");
                }

                // Verificar si existe el tipo de documento
                var cmdCheckDoc = new MySqlCommand(
                    "SELECT COUNT(*) FROM TipoDocumento WHERE id = @tipo_doc_id", conn, transaction);
                cmdCheckDoc.Parameters.AddWithValue("@tipo_doc_id", proveedor.TipoDocumentoId);
                var docExists = Convert.ToInt32(await cmdCheckDoc.ExecuteScalarAsync()) > 0;

                if (!docExists)
                {
                    throw new Exception($"El tipo de documento con ID {proveedor.TipoDocumentoId} no existe. Los tipos válidos son: 1 (Cédula de Ciudadanía), 2 (Cédula de Extranjería), 3 (Pasaporte), 4 (Tarjeta de Identidad), 5 (NIT)");
                }

                // Verificar si existe la ciudad
                var cmdCheckCiudad = new MySqlCommand(
                    "SELECT COUNT(*) FROM Ciudad WHERE id = @ciudad_id", conn, transaction);
                cmdCheckCiudad.Parameters.AddWithValue("@ciudad_id", proveedor.CiudadId);
                var ciudadExists = Convert.ToInt32(await cmdCheckCiudad.ExecuteScalarAsync()) > 0;

                if (!ciudadExists)
                {
                    throw new Exception($"La ciudad con ID {proveedor.CiudadId} no existe. Las ciudades válidas son del 1 al 20");
                }

                // Generar ID único para el tercero
                var terceroId = Guid.NewGuid().ToString("N").Substring(0, 20);

                // Primero insertar en Terceros
                var cmdTercero = new MySqlCommand(
                    @"INSERT INTO Terceros (id, nombre, apellidos, email, tipo_doc_id, tipo_tercero_id, ciudad_id) 
                    VALUES (@id, @nombre, @apellidos, @email, @tipo_doc_id, @tipo_tercero_id, @ciudad_id)", conn, transaction);
                
                cmdTercero.Parameters.AddWithValue("@id", terceroId);
                cmdTercero.Parameters.AddWithValue("@nombre", proveedor.Nombre);
                cmdTercero.Parameters.AddWithValue("@apellidos", proveedor.Apellidos);
                cmdTercero.Parameters.AddWithValue("@email", proveedor.Email);
                cmdTercero.Parameters.AddWithValue("@tipo_doc_id", proveedor.TipoDocumentoId);
                cmdTercero.Parameters.AddWithValue("@tipo_tercero_id", proveedor.TipoTerceroId);
                cmdTercero.Parameters.AddWithValue("@ciudad_id", proveedor.CiudadId);

                await cmdTercero.ExecuteNonQueryAsync();

                // Luego insertar en Proveedor
                var cmdProveedor = new MySqlCommand(
                    @"INSERT INTO Proveedor (tercero_id, fecha_ingreso, descuento) 
                    VALUES (@tercero_id, @fecha_ingreso, @descuento)", conn, transaction);

                cmdProveedor.Parameters.AddWithValue("@tercero_id", terceroId);
                cmdProveedor.Parameters.AddWithValue("@fecha_ingreso", proveedor.FechaIngreso.Date);
                cmdProveedor.Parameters.AddWithValue("@descuento", proveedor.Descuento);

                await cmdProveedor.ExecuteNonQueryAsync();
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
                FROM Producto p 
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