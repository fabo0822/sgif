using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sgif.domain.entities;
using sgif.domain.ports;
using MySql.Data.MySqlClient;

namespace sgif.application.services
{
    public class ProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly ITerceroRepository _terceroRepository;
        private readonly string _connectionString;

        public ProveedorService(IProveedorRepository proveedorRepository, ITerceroRepository terceroRepository, string connectionString)
        {
            _proveedorRepository = proveedorRepository;
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
            
            var command = new MySqlCommand("SELECT id, descripcion FROM TipoTercero WHERE id IN (3, 6, 7, 12, 13, 18) ORDER BY descripcion", connection);
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

        private async Task MostrarTercerosDisponibles()
        {
            Console.WriteLine("\n=== TERCEROS DISPONIBLES ===");
            var terceros = await _terceroRepository.GetAllAsync();
            foreach (var tercero in terceros)
            {
                Console.WriteLine($"ID: {tercero.Id}, Nombre: {tercero.Nombre}");
            }
        }

        public async Task RegistrarProveedor()
        {
            try
            {
                await MostrarTercerosDisponibles();

                Console.Write("Ingrese el ID del tercero: ");
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

                var proveedor = new Proveedor
                {
                    TerceroId = terceroId
                };

                await _proveedorRepository.AddAsync(proveedor);
                Console.WriteLine("\n✅ Proveedor registrado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al registrar proveedor: {ex.Message}");
            }
        }

        public async Task ListarProveedores()
        {
            try
            {
                var proveedores = await _proveedorRepository.GetAllAsync();
                if (proveedores == null || !proveedores.Any())
                {
                    Console.WriteLine("\nNo hay proveedores registrados.");
                    return;
                }

                Console.WriteLine("\n=== LISTA DE PROVEEDORES ===");
                foreach (var proveedor in proveedores)
                {
                    var tercero = await _terceroRepository.GetById(proveedor.TerceroId);
                    if (tercero == null)
                    {
                        Console.WriteLine($"Proveedor ID: {proveedor.Id} - Tercero no encontrado");
                        continue;
                    }

                    Console.WriteLine($"ID: {proveedor.Id}");
                    Console.WriteLine($"Nombre: {tercero.Nombre?? "N/A"} {tercero.Apellidos?? "N/A"}");
                    Console.WriteLine($"Email: {tercero.Email?? "N/A"}");
                    Console.WriteLine($"Fecha de Ingreso: {proveedor.FechaIngreso:dd/MM/yyyy}");
                    Console.WriteLine($"Descuento: {proveedor.Descuento:P}");
                    Console.WriteLine("------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al listar proveedores: {ex.Message}");
            }
        }

        public async Task ActualizarProveedor()
        {
            Console.WriteLine("\n=== ACTUALIZAR PROVEEDOR ===");

            Console.Write("ID del Proveedor: ");
            if (!int.TryParse(Console.ReadLine(), out int proveedorId))
            {
                Console.WriteLine("❌ ID del Proveedor inválido.");
                return;
            }

            var proveedor = await _proveedorRepository.GetByIdAsync(proveedorId);
            if (proveedor == null)
            {
                Console.WriteLine("❌ Proveedor no encontrado.");
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
            proveedor.TerceroId = terceroId;

            await _proveedorRepository.UpdateAsync(proveedor);
            Console.WriteLine("\n✅ Proveedor actualizado exitosamente.");
        }

        public async Task EliminarProveedor()
        {
            Console.WriteLine("\n=== ELIMINAR PROVEEDOR ===");

            Console.Write("ID del Proveedor: ");
            if (!int.TryParse(Console.ReadLine(), out int proveedorId))
            {
                Console.WriteLine("❌ ID del Proveedor inválido.");
                return;
            }

            var proveedor = await _proveedorRepository.GetByIdAsync(proveedorId);
            if (proveedor == null)
            {
                Console.WriteLine("❌ Proveedor no encontrado.");
                return;
            }

            await _proveedorRepository.DeleteAsync(proveedorId);
            Console.WriteLine("\n✅ Proveedor eliminado exitosamente.");
        }

        public async Task VerProductosProveedor()
        {
            Console.Clear();
            Console.WriteLine("=== PRODUCTOS DEL PROVEEDOR ===");

            Console.Write("Ingrese el ID del proveedor: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ ID inválido.");
                return;
            }

            try
            {
                var productos = await _proveedorRepository.GetProductosAsync(id);
                if (!productos.Any())
                {
                    Console.WriteLine("El proveedor no tiene productos asociados.");
                }
                else
                {
                    foreach (var p in productos)
                    {
                        Console.WriteLine($"\nID: {p.Id}");
                        Console.WriteLine($"Nombre: {p.Nombre}");
                        Console.WriteLine($"Stock: {p.Stock}");
                        Console.WriteLine($"Stock Mínimo: {p.StockMin}");
                        Console.WriteLine($"Stock Máximo: {p.StockMax}");
                        Console.WriteLine("------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al obtener productos: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}