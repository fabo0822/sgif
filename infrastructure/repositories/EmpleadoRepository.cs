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
                JOIN Terceros t ON e.tercero_id = t.id 
                WHERE e.id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Empleado
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroId = reader.GetString(reader.GetOrdinal("tercero_id")),
                    FechaIngreso = reader.GetDateTime(reader.GetOrdinal("fecha_ingreso")),
                    SalarioBase = reader.GetDecimal(reader.GetOrdinal("salario_base")),
                    EpsId = reader.GetInt32(reader.GetOrdinal("eps_id")),
                    ArlId = reader.GetInt32(reader.GetOrdinal("arl_id")),
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

        public async Task<IEnumerable<Empleado>> GetAllAsync()
        {
            var empleados = new List<Empleado>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                @"SELECT e.*, t.nombre, t.apellidos, t.email, t.tipo_doc_id, t.tipo_tercero_id, t.ciudad_id 
                FROM Empleado e 
                JOIN Terceros t ON e.tercero_id = t.id", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                empleados.Add(new Empleado
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroId = reader.GetString(reader.GetOrdinal("tercero_id")),
                    FechaIngreso = reader.GetDateTime(reader.GetOrdinal("fecha_ingreso")),
                    SalarioBase = reader.GetDecimal(reader.GetOrdinal("salario_base")),
                    EpsId = reader.GetInt32(reader.GetOrdinal("eps_id")),
                    ArlId = reader.GetInt32(reader.GetOrdinal("arl_id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Apellidos = reader.GetString(reader.GetOrdinal("apellidos")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    TipoDocumentoId = reader.GetInt32(reader.GetOrdinal("tipo_doc_id")),
                    TipoTerceroId = reader.GetInt32(reader.GetOrdinal("tipo_tercero_id")),
                    CiudadId = reader.GetInt32(reader.GetOrdinal("ciudad_id"))
                });
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
                var command = new MySqlCommand("CALL insertar_tercero_y_empleado(@id, @nombre, @apellidos, @email, @tipo_doc_id, @tipo_tercero_id, @ciudad_id, @fecha_ingreso, @salario_base, @eps_id, @arl_id)", conn, transaction);

                var terceroId = Guid.NewGuid().ToString("N").Substring(0, 20);
                command.Parameters.AddWithValue("@id", terceroId);
                command.Parameters.AddWithValue("@nombre", empleado.Nombre);
                command.Parameters.AddWithValue("@apellidos", empleado.Apellidos);
                command.Parameters.AddWithValue("@email", empleado.Email);
                command.Parameters.AddWithValue("@tipo_doc_id", empleado.TipoDocumentoId);
                command.Parameters.AddWithValue("@tipo_tercero_id", 2); // Tipo de tercero para empleados
                command.Parameters.AddWithValue("@ciudad_id", empleado.CiudadId);
                command.Parameters.AddWithValue("@fecha_ingreso", empleado.FechaIngreso);
                command.Parameters.AddWithValue("@salario_base", empleado.SalarioBase);
                command.Parameters.AddWithValue("@eps_id", empleado.EpsId);
                command.Parameters.AddWithValue("@arl_id", empleado.ArlId);

                await command.ExecuteNonQueryAsync();
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