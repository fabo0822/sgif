using sgif.domain.entities;
using sgif.domain.ports;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sgif.infrastructure.repositories
{
    public class TerceroRepository : ITerceroRepository
    {
        private readonly string _connectionString;

        public TerceroRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Tercero>> GetAll()
        {
            var terceros = new List<Tercero>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Terceros", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                terceros.Add(new Tercero
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("id")) ? string.Empty : reader.GetString(reader.GetOrdinal("id")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombre")),
                    Apellidos = reader.IsDBNull(reader.GetOrdinal("apellidos")) ? string.Empty : reader.GetString(reader.GetOrdinal("apellidos")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                    TipoDocumentoId = reader.IsDBNull(reader.GetOrdinal("tipo_doc_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("tipo_doc_id")),
                    TipoTerceroId = reader.IsDBNull(reader.GetOrdinal("tipo_tercero_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("tipo_tercero_id")),
                    CiudadId = reader.IsDBNull(reader.GetOrdinal("ciudad_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("ciudad_id"))
                });
            }

            return terceros;
        }

        public async Task<Tercero> GetById(string id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Terceros WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Tercero
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("id")) ? string.Empty : reader.GetString(reader.GetOrdinal("id")),
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

        public async Task Add(Tercero tercero)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(
                "INSERT INTO Terceros (id, nombre, apellidos, email, tipo_doc_id, tipo_tercero_id, ciudad_id) " +
                "VALUES (@id, @nombre, @apellidos, @email, @tipo_doc_id, @tipo_tercero_id, @ciudad_id)",
                conn);

            cmd.Parameters.AddWithValue("@id", tercero.Id);
            cmd.Parameters.AddWithValue("@nombre", tercero.Nombre);
            cmd.Parameters.AddWithValue("@apellidos", tercero.Apellidos);
            cmd.Parameters.AddWithValue("@email", tercero.Email);
            cmd.Parameters.AddWithValue("@tipo_doc_id", tercero.TipoDocumentoId);
            cmd.Parameters.AddWithValue("@tipo_tercero_id", tercero.TipoTerceroId);
            cmd.Parameters.AddWithValue("@ciudad_id", tercero.CiudadId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Update(Tercero tercero)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(
                "UPDATE Terceros SET nombre = @nombre, apellidos = @apellidos, email = @email, " +
                "tipo_doc_id = @tipo_doc_id, tipo_tercero_id = @tipo_tercero_id, ciudad_id = @ciudad_id " +
                "WHERE id = @id",
                conn);

            cmd.Parameters.AddWithValue("@id", tercero.Id);
            cmd.Parameters.AddWithValue("@nombre", tercero.Nombre);
            cmd.Parameters.AddWithValue("@apellidos", tercero.Apellidos);
            cmd.Parameters.AddWithValue("@email", tercero.Email);
            cmd.Parameters.AddWithValue("@tipo_doc_id", tercero.TipoDocumentoId);
            cmd.Parameters.AddWithValue("@tipo_tercero_id", tercero.TipoTerceroId);
            cmd.Parameters.AddWithValue("@ciudad_id", tercero.CiudadId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(string id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("DELETE FROM Terceros WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        // Implementación de IGenericRepository<Tercero>
        public async Task<Tercero> GetById(int id)
        {
            return await GetById(id.ToString());
        }

        public async Task Delete(int id)
        {
            await Delete(id.ToString());
        }

        public async Task<IEnumerable<Tercero>> GetAllAsync()
        {
            var terceros = new List<Tercero>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                @"SELECT id, nombre, apellidos, email, tipo_doc_id, tipo_tercero_id, ciudad_id 
                FROM Terceros", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tercero = new Tercero
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("id")) ? string.Empty : reader.GetString(reader.GetOrdinal("id")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombre")),
                    Apellidos = reader.IsDBNull(reader.GetOrdinal("apellidos")) ? string.Empty : reader.GetString(reader.GetOrdinal("apellidos")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                    TipoDocumentoId = reader.IsDBNull(reader.GetOrdinal("tipo_doc_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("tipo_doc_id")),
                    TipoTerceroId = reader.IsDBNull(reader.GetOrdinal("tipo_tercero_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("tipo_tercero_id")),
                    CiudadId = reader.IsDBNull(reader.GetOrdinal("ciudad_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("ciudad_id"))
                };
                terceros.Add(tercero);
            }
            return terceros;
        }

        public async Task<Tercero?> GetByIdAsync(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                @"SELECT id, nombre, apellidos, email, tipo_doc_id, tipo_tercero_id, ciudad_id 
                FROM Terceros 
                WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Tercero
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("id")) ? string.Empty : reader.GetString(reader.GetOrdinal("id")),
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
    }
}