using MySql.Data.MySqlClient;
using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.infrastructure.repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly string _connectionString;

        public EmpleadoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Empleado?> GetByIdAsync(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                @"SELECT e.*, t.nombre, t.apellidos, t.email, t.tipo_doc_id, t.tipo_tercero_id, t.ciudad_id 
                FROM Empleado e 
                LEFT JOIN Terceros t ON e.tercero_id = t.id 
                WHERE e.id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Empleado
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroId = reader.IsDBNull(reader.GetOrdinal("tercero_id")) ? string.Empty : reader.GetString(reader.GetOrdinal("tercero_id")),
                    FechaIngreso = reader.IsDBNull(reader.GetOrdinal("fecha_ingreso")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("fecha_ingreso")),
                    SalarioBase = reader.IsDBNull(reader.GetOrdinal("salario_base")) ? 0 : reader.GetDecimal(reader.GetOrdinal("salario_base")),
                    EpsId = reader.IsDBNull(reader.GetOrdinal("eps_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("eps_id")),
                    ArlId = reader.IsDBNull(reader.GetOrdinal("arl_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("arl_id")),
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

        public async Task<IEnumerable<Empleado>> GetAllAsync()
        {
            var empleados = new List<Empleado>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                @"SELECT e.*, t.nombre, t.apellidos, t.email, t.tipo_doc_id, t.tipo_tercero_id, t.ciudad_id 
                FROM Empleado e 
                LEFT JOIN Terceros t ON e.tercero_id = t.id", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var empleado = new Empleado
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroId = reader.IsDBNull(reader.GetOrdinal("tercero_id")) ? string.Empty : reader.GetString(reader.GetOrdinal("tercero_id")),
                    FechaIngreso = reader.IsDBNull(reader.GetOrdinal("fecha_ingreso")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("fecha_ingreso")),
                    SalarioBase = reader.IsDBNull(reader.GetOrdinal("salario_base")) ? 0 : reader.GetDecimal(reader.GetOrdinal("salario_base")),
                    EpsId = reader.IsDBNull(reader.GetOrdinal("eps_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("eps_id")),
                    ArlId = reader.IsDBNull(reader.GetOrdinal("arl_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("arl_id")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombre")),
                    Apellidos = reader.IsDBNull(reader.GetOrdinal("apellidos")) ? string.Empty : reader.GetString(reader.GetOrdinal("apellidos")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                    TipoDocumentoId = reader.IsDBNull(reader.GetOrdinal("tipo_doc_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("tipo_doc_id")),
                    TipoTerceroId = reader.IsDBNull(reader.GetOrdinal("tipo_tercero_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("tipo_tercero_id")),
                    CiudadId = reader.IsDBNull(reader.GetOrdinal("ciudad_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("ciudad_id"))
                };
                empleados.Add(empleado);
            }
            return empleados;
        }

        public async Task AddAsync(Empleado empleado)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                // Verificar si existe el tipo de documento
                var cmdCheckTipoDoc = new MySqlCommand(
                    "SELECT COUNT(*) FROM TipoDocumento WHERE id = @tipo_doc_id", conn, transaction);
                cmdCheckTipoDoc.Parameters.AddWithValue("@tipo_doc_id", empleado.TipoDocumentoId);
                var tipoDocExists = Convert.ToInt32(await cmdCheckTipoDoc.ExecuteScalarAsync()) > 0;

                if (!tipoDocExists)
                {
                    throw new Exception($"El tipo de documento con ID {empleado.TipoDocumentoId} no existe.");
                }

                // Verificar si existe el tipo de tercero para empleados (id = 2)
                var cmdCheckTipo = new MySqlCommand(
                    "SELECT COUNT(*) FROM TipoTercero WHERE id = 2", conn, transaction);
                var tipoExists = Convert.ToInt32(await cmdCheckTipo.ExecuteScalarAsync()) > 0;

                if (!tipoExists)
                {
                    // Insertar el tipo de tercero si no existe
                    var cmdInsertTipo = new MySqlCommand(
                        "INSERT INTO TipoTercero (id, descripcion) VALUES (2, 'Empleado')", conn, transaction);
                    await cmdInsertTipo.ExecuteNonQueryAsync();
                }

                // Verificar si existe la ciudad
                var cmdCheckCiudad = new MySqlCommand(
                    "SELECT COUNT(*) FROM Ciudad WHERE id = @ciudad_id", conn, transaction);
                cmdCheckCiudad.Parameters.AddWithValue("@ciudad_id", empleado.CiudadId);
                var ciudadExists = Convert.ToInt32(await cmdCheckCiudad.ExecuteScalarAsync()) > 0;

                if (!ciudadExists)
                {
                    throw new Exception($"La ciudad con ID {empleado.CiudadId} no existe.");
                }

                // Primero insertar en Terceros
                var cmdTercero = new MySqlCommand(
                    @"INSERT INTO Terceros (id, nombre, apellidos, email, tipo_doc_id, tipo_tercero_id, ciudad_id) 
                    VALUES (@id, @nombre, @apellidos, @email, @tipo_doc_id, 2, @ciudad_id)", conn, transaction);
                
                var terceroId = Guid.NewGuid().ToString("N").Substring(0, 20);
                cmdTercero.Parameters.AddWithValue("@id", terceroId);
                cmdTercero.Parameters.AddWithValue("@nombre", empleado.Nombre);
                cmdTercero.Parameters.AddWithValue("@apellidos", empleado.Apellidos);
                cmdTercero.Parameters.AddWithValue("@email", empleado.Email);
                cmdTercero.Parameters.AddWithValue("@tipo_doc_id", empleado.TipoDocumentoId);
                cmdTercero.Parameters.AddWithValue("@ciudad_id", empleado.CiudadId);

                await cmdTercero.ExecuteNonQueryAsync();

                // Luego insertar en Empleado
                var cmdEmpleado = new MySqlCommand(
                    @"INSERT INTO Empleado (tercero_id, fecha_ingreso, salario_base, eps_id, arl_id) 
                    VALUES (@tercero_id, @fecha_ingreso, @salario_base, @eps_id, @arl_id)", conn, transaction);

                cmdEmpleado.Parameters.AddWithValue("@tercero_id", terceroId);
                cmdEmpleado.Parameters.AddWithValue("@fecha_ingreso", empleado.FechaIngreso);
                cmdEmpleado.Parameters.AddWithValue("@salario_base", empleado.SalarioBase);
                cmdEmpleado.Parameters.AddWithValue("@eps_id", empleado.EpsId);
                cmdEmpleado.Parameters.AddWithValue("@arl_id", empleado.ArlId);

                await cmdEmpleado.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al registrar empleado: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(Empleado empleado)
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

                cmdTercero.Parameters.AddWithValue("@id", empleado.TerceroId);
                cmdTercero.Parameters.AddWithValue("@nombre", empleado.Nombre);
                cmdTercero.Parameters.AddWithValue("@apellidos", empleado.Apellidos);
                cmdTercero.Parameters.AddWithValue("@email", empleado.Email);
                cmdTercero.Parameters.AddWithValue("@tipo_doc_id", empleado.TipoDocumentoId);
                cmdTercero.Parameters.AddWithValue("@ciudad_id", empleado.CiudadId);

                await cmdTercero.ExecuteNonQueryAsync();

                // Actualizar Empleado
                var cmdEmpleado = new MySqlCommand(
                    @"UPDATE Empleado 
                    SET salario_base = @salario_base, eps_id = @eps_id, arl_id = @arl_id 
                    WHERE id = @id", conn, transaction);

                cmdEmpleado.Parameters.AddWithValue("@id", empleado.Id);
                cmdEmpleado.Parameters.AddWithValue("@salario_base", empleado.SalarioBase);
                cmdEmpleado.Parameters.AddWithValue("@eps_id", empleado.EpsId);
                cmdEmpleado.Parameters.AddWithValue("@arl_id", empleado.ArlId);

                await cmdEmpleado.ExecuteNonQueryAsync();
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
                    "SELECT tercero_id FROM Empleado WHERE id = @id", conn, transaction);
                cmdGetTercero.Parameters.AddWithValue("@id", id);

                var terceroId = (string?)await cmdGetTercero.ExecuteScalarAsync();
                if (terceroId == null) return;

                // Eliminar Empleado
                var cmdEmpleado = new MySqlCommand(
                    "DELETE FROM Empleado WHERE id = @id", conn, transaction);
                cmdEmpleado.Parameters.AddWithValue("@id", id);
                await cmdEmpleado.ExecuteNonQueryAsync();

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
    }
} 