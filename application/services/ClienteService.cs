using sgif.domain.entities;
using sgif.domain.ports;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sgif.application.services
{
    public class ClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ITerceroRepository _terceroRepository;
        private readonly string _connectionString;

        public ClienteService(IClienteRepository clienteRepository, ITerceroRepository terceroRepository, string connectionString)
        {
            _clienteRepository = clienteRepository;
            _terceroRepository = terceroRepository;
            _connectionString = connectionString;
        }

        private async Task<List<(int Id, string Nombre)>> ObtenerCiudadesAsync()
        {
            var ciudades = new List<(int Id, string Nombre)>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand(@"
                SELECT c.id, c.nombre, r.nombre as region, p.nombre as pais 
                FROM Ciudad c 
                JOIN Region r ON c.region_id = r.id 
                JOIN Pais p ON r.pais_id = p.id 
                ORDER BY p.nombre, r.nombre, c.nombre", connection);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ciudades.Add((
                    Convert.ToInt32(reader["id"]),
                    $"{reader["nombre"]} - {reader["region"]} - {reader["pais"]}"
                ));
            }
            return ciudades;
        }

        private async Task<List<(int Id, string Descripcion)>> ObtenerTiposDocumentoAsync()
        {
            var tipos = new List<(int Id, string Descripcion)>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("SELECT id, descripcion FROM TipoDocumento ORDER BY descripcion", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tipos.Add((
                    Convert.ToInt32(reader["id"]),
                    reader["descripcion"].ToString() ?? ""
                ));
            }
            return tipos;
        }

        private async Task<List<(int Id, string Descripcion)>> ObtenerTiposTerceroAsync()
        {
            var tipos = new List<(int Id, string Descripcion)>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = new MySqlCommand("SELECT id, descripcion FROM TipoTercero ORDER BY descripcion", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tipos.Add((
                    Convert.ToInt32(reader["id"]),
                    reader["descripcion"].ToString() ?? ""
                ));
            }
            return tipos;
        }

        public async Task ListarClientes()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            foreach (var cliente in clientes)
            {
                Console.WriteLine($"ID: {cliente.Id}");
                Console.WriteLine($"Tercero ID: {cliente.TerceroId}");
                Console.WriteLine("------------------------");
            }
        }

        public async Task RegistrarCliente()
        {
            Console.WriteLine("\n=== REGISTRAR NUEVO CLIENTE ===");

            var cliente = new Cliente();

            Console.Write("ID del Tercero: ");
            var terceroId = Console.ReadLine();
            if (string.IsNullOrEmpty(terceroId))
            {
                Console.WriteLine("❌ ID del Tercero no puede estar vacío.");
                return;
            }

            var tercero = await _terceroRepository.GetById(terceroId);
            if (tercero == null)
            {
                Console.WriteLine("❌ Tercero no encontrado.");
                return;
            }
            cliente.TerceroId = terceroId;

            await _clienteRepository.AddAsync(cliente);
            Console.WriteLine("\n✅ Cliente registrado exitosamente.");
        }

        public async Task ActualizarCliente()
        {
            Console.WriteLine("\n=== ACTUALIZAR CLIENTE ===");

            Console.Write("ID del Cliente: ");
            if (!int.TryParse(Console.ReadLine(), out int clienteId))
            {
                Console.WriteLine("❌ ID del Cliente inválido.");
                return;
            }

            var cliente = await _clienteRepository.GetByIdAsync(clienteId);
            if (cliente == null)
            {
                Console.WriteLine("❌ Cliente no encontrado.");
                return;
            }

            Console.Write("Nuevo ID del Tercero: ");
            var terceroId = Console.ReadLine();
            if (string.IsNullOrEmpty(terceroId))
            {
                Console.WriteLine("❌ ID del Tercero no puede estar vacío.");
                return;
            }

            var tercero = await _terceroRepository.GetById(terceroId);
            if (tercero == null)
            {
                Console.WriteLine("❌ Tercero no encontrado.");
                return;
            }
            cliente.TerceroId = terceroId;

            await _clienteRepository.UpdateAsync(cliente);
            Console.WriteLine("\n✅ Cliente actualizado exitosamente.");
        }

        public async Task EliminarCliente()
        {
            Console.WriteLine("\n=== ELIMINAR CLIENTE ===");

            Console.Write("ID del Cliente: ");
            if (!int.TryParse(Console.ReadLine(), out int clienteId))
            {
                Console.WriteLine("❌ ID del Cliente inválido.");
                return;
            }

            var cliente = await _clienteRepository.GetByIdAsync(clienteId);
            if (cliente == null)
            {
                Console.WriteLine("❌ Cliente no encontrado.");
                return;
            }

            await _clienteRepository.DeleteAsync(clienteId);
            Console.WriteLine("\n✅ Cliente eliminado exitosamente.");
        }
    }
}