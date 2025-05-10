using MySql.Data.MySqlClient;
using sgif.domain.entities;
using sgif.domain.ports;
using sgif.infrastructure.mysql;

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
            
            var command = new MySqlCommand("SELECT * FROM clientes WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Cliente
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Direccion = reader.GetString(reader.GetOrdinal("direccion")),
                    Ciudad = reader.GetString(reader.GetOrdinal("ciudad")),
                    Telefono = reader.GetString(reader.GetOrdinal("telefono")),
                    Correo = reader.GetString(reader.GetOrdinal("correo"))
                };
            }
            return null;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            var clientes = new List<Cliente>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("SELECT * FROM clientes", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                clientes.Add(new Cliente
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Direccion = reader.GetString(reader.GetOrdinal("direccion")),
                    Ciudad = reader.GetString(reader.GetOrdinal("ciudad")),
                    Telefono = reader.GetString(reader.GetOrdinal("telefono")),
                    Correo = reader.GetString(reader.GetOrdinal("correo"))
                });
            }
            return clientes;
        }

        public async Task AddAsync(Cliente cliente)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand(
                "INSERT INTO clientes (nombre, direccion, ciudad, telefono, correo) VALUES (@nombre, @direccion, @ciudad, @telefono, @correo)", 
                connection);
            command.Parameters.AddWithValue("@nombre", cliente.Nombre);
            command.Parameters.AddWithValue("@direccion", cliente.Direccion);
            command.Parameters.AddWithValue("@ciudad", cliente.Ciudad);
            command.Parameters.AddWithValue("@telefono", cliente.Telefono);
            command.Parameters.AddWithValue("@correo", cliente.Correo);
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand(
                "UPDATE clientes SET nombre = @nombre, direccion = @direccion, ciudad = @ciudad, telefono = @telefono, correo = @correo WHERE id = @id", 
                connection);
            command.Parameters.AddWithValue("@id", cliente.Id);
            command.Parameters.AddWithValue("@nombre", cliente.Nombre);
            command.Parameters.AddWithValue("@direccion", cliente.Direccion);
            command.Parameters.AddWithValue("@ciudad", cliente.Ciudad);
            command.Parameters.AddWithValue("@telefono", cliente.Telefono);
            command.Parameters.AddWithValue("@correo", cliente.Correo);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("DELETE FROM clientes WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();
        }

        public void Crear(Cliente entity)
        {
            using var conexion = new MySqlConnection(_connectionString);
            conexion.Open();
            var comando = new MySqlCommand("INSERT INTO clientes (nombre, direccion, ciudad, telefono, correo) VALUES (@nombre, @direccion, @ciudad, @telefono, @correo)", conexion);
            comando.Parameters.AddWithValue("@nombre", entity.Nombre);
            comando.Parameters.AddWithValue("@direccion", entity.Direccion);
            comando.Parameters.AddWithValue("@ciudad", entity.Ciudad);
            comando.Parameters.AddWithValue("@telefono", entity.Telefono);
            comando.Parameters.AddWithValue("@correo", entity.Correo);
            comando.ExecuteNonQuery();
        }

        public void Actualizar(Cliente entity)
        {
            using var conexion = new MySqlConnection(_connectionString);
            conexion.Open();
            var comando = new MySqlCommand("UPDATE clientes SET nombre = @nombre, direccion = @direccion, ciudad = @ciudad, telefono = @telefono, correo = @correo WHERE id = @id", conexion);
            comando.Parameters.AddWithValue("@id", entity.Id);
            comando.Parameters.AddWithValue("@nombre", entity.Nombre);
            comando.Parameters.AddWithValue("@direccion", entity.Direccion);
            comando.Parameters.AddWithValue("@ciudad", entity.Ciudad);
            comando.Parameters.AddWithValue("@telefono", entity.Telefono);
            comando.Parameters.AddWithValue("@correo", entity.Correo);
            comando.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var conexion = new MySqlConnection(_connectionString);
            conexion.Open();
            var comando = new MySqlCommand("DELETE FROM clientes WHERE id = @id", conexion);
            comando.Parameters.AddWithValue("@id", id);
            comando.ExecuteNonQuery();
        }

        public List<Cliente> ObtenerTodos()
        {
            var lista = new List<Cliente>();
            using var conexion = new MySqlConnection(_connectionString);
            conexion.Open();
            var comando = new MySqlCommand("SELECT * FROM clientes", conexion);
            using var reader = comando.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Cliente
                {
                    Id = reader.GetInt32("id"),
                    Nombre = reader.GetString("nombre"),
                    Direccion = reader.GetString("direccion"),
                    Ciudad = reader.GetString("ciudad"),
                    Telefono = reader.GetString("telefono"),
                    Correo = reader.GetString("correo")
                });
            }
            return lista;
        }

        public Cliente? ObtenerPorId(int id)
        {
            using var conexion = new MySqlConnection(_connectionString);
            conexion.Open();
            var comando = new MySqlCommand("SELECT * FROM clientes WHERE id = @id", conexion);
            comando.Parameters.AddWithValue("@id", id);
            using var reader = comando.ExecuteReader();
            if (reader.Read())
            {
                return new Cliente
                {
                    Id = reader.GetInt32("id"),
                    Nombre = reader.GetString("nombre"),
                    Direccion = reader.GetString("direccion"),
                    Ciudad = reader.GetString("ciudad"),
                    Telefono = reader.GetString("telefono"),
                    Correo = reader.GetString("correo")
                };
            }
            return null;
        }
    }
} 