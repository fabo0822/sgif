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
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    // Tipo = reader.GetString(reader.GetOrdinal("tipo")),
                    // Documento = reader.GetString(reader.GetOrdinal("documento")),
                    Direccion = reader.GetString(reader.GetOrdinal("direccion")),
                    Telefono = reader.GetString(reader.GetOrdinal("telefono")),
                    Email = reader.GetString(reader.GetOrdinal("email"))
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
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    // Tipo = reader.GetString(reader.GetOrdinal("tipo")),
                    // Documento = reader.GetString(reader.GetOrdinal("documento")),
                    Direccion = reader.GetString(reader.GetOrdinal("direccion")),
                    Telefono = reader.GetString(reader.GetOrdinal("telefono")),
                    Email = reader.GetString(reader.GetOrdinal("email"))
                };
            }

            return new Tercero(); // Reemplazar el retorno nulo con una instancia vacía de Tercero
        }

        public async Task Add(Tercero tercero)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(
                "INSERT INTO Terceros (id, nombre, tipo, direccion, telefono, email) " +
                "VALUES (@id, @nombre, @tipo, @direccion, @telefono, @email)",
                conn);

            cmd.Parameters.AddWithValue("@id", tercero.Id);
            cmd.Parameters.AddWithValue("@nombre", tercero.Nombre);
            // cmd.Parameters.AddWithValue("@tipo", tercero.Tipo);
            // cmd.Parameters.AddWithValue("@documento", tercero.Documento);
            cmd.Parameters.AddWithValue("@direccion", tercero.Direccion);
            cmd.Parameters.AddWithValue("@telefono", tercero.Telefono);
            cmd.Parameters.AddWithValue("@email", tercero.Email);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Update(Tercero tercero)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(
                "UPDATE Terceros SET nombre = @nombre, tipo = @tipo, direccion = @direccion, " +
                "telefono = @telefono, email = @email WHERE id = @id",
                conn);

            cmd.Parameters.AddWithValue("@id", tercero.Id);
            cmd.Parameters.AddWithValue("@nombre", tercero.Nombre);
            // cmd.Parameters.AddWithValue("@tipo", tercero.Tipo);
            // cmd.Parameters.AddWithValue("@documento", tercero.Documento);
            cmd.Parameters.AddWithValue("@direccion", tercero.Direccion);
            cmd.Parameters.AddWithValue("@telefono", tercero.Telefono);
            cmd.Parameters.AddWithValue("@email", tercero.Email);

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
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand("SELECT * FROM Terceros", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                terceros.Add(new Tercero
                {
                    Id = reader["id"]?.ToString() ?? string.Empty,
                    Nombre = reader["nombre"]?.ToString() ?? string.Empty
                });
            }

            return terceros;
        }
    }
}