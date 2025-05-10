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
        private readonly IProveedorRepository _repository;
        private readonly string _connectionString;

        public ProveedorService(IProveedorRepository repository, string connectionString)
        {
            _repository = repository;
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

        public async Task RegistrarProveedor()
        {
            Console.WriteLine("\n=== Registro de Proveedor ===");

            // Validar y obtener nombre
            string nombre;
            do
            {
                Console.Write("Ingrese el nombre del proveedor: ");
                nombre = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.WriteLine("Error: El nombre no puede estar vacío.");
                }
            } while (string.IsNullOrWhiteSpace(nombre));

            // Validar y obtener apellidos
            string apellidos;
            do
            {
                Console.Write("Ingrese los apellidos del proveedor: ");
                apellidos = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(apellidos))
                {
                    Console.WriteLine("Error: Los apellidos no pueden estar vacíos.");
                }
            } while (string.IsNullOrWhiteSpace(apellidos));

            // Validar y obtener email
            string email;
            bool emailValido = false;
            do
            {
                Console.Write("Ingrese el email del proveedor: ");
                email = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Error: El email no puede estar vacío.");
                    continue;
                }
                if (!IsValidEmail(email))
                {
                    Console.WriteLine("Error: El formato del email no es válido.");
                    continue;
                }
                if (await EmailExistsAsync(email))
                {
                    Console.WriteLine("Error: Este email ya está registrado.");
                    continue;
                }
                emailValido = true;
            } while (!emailValido);

            // Mostrar y validar tipo de documento
            Console.WriteLine("\nTipos de documento disponibles:");
            Console.WriteLine("1. Cédula de Ciudadanía");
            Console.WriteLine("2. Cédula de Extranjería");
            Console.WriteLine("3. Pasaporte");
            Console.WriteLine("4. Tarjeta de Identidad");
            Console.WriteLine("5. NIT");

            int tipoDocumentoId;
            do
            {
                Console.Write("\nSeleccione el tipo de documento (1-5): ");
                if (!int.TryParse(Console.ReadLine(), out tipoDocumentoId) || tipoDocumentoId < 1 || tipoDocumentoId > 5)
                {
                    Console.WriteLine("Error: Por favor seleccione un número válido entre 1 y 5.");
                }
            } while (tipoDocumentoId < 1 || tipoDocumentoId > 5);

            // Mostrar y validar tipo de proveedor
            Console.WriteLine("\nTipos de proveedor disponibles:");
            Console.WriteLine("3. Proveedor");
            Console.WriteLine("6. Proveedor Premium");
            Console.WriteLine("7. Proveedor Local");
            Console.WriteLine("12. Proveedor Internacional");
            Console.WriteLine("13. Proveedor Nacional");
            Console.WriteLine("18. Proveedor Exclusivo");

            int tipoProveedorId;
            do
            {
                Console.Write("\nSeleccione el tipo de proveedor (3,6,7,12,13,18): ");
                if (!int.TryParse(Console.ReadLine(), out tipoProveedorId) || 
                    !new[] { 3, 6, 7, 12, 13, 18 }.Contains(tipoProveedorId))
                {
                    Console.WriteLine("Error: Por favor seleccione un tipo de proveedor válido.");
                }
            } while (!new[] { 3, 6, 7, 12, 13, 18 }.Contains(tipoProveedorId));

            // Mostrar y validar ciudad
            Console.WriteLine("\nCiudades disponibles:");
            Console.WriteLine("1. Medellín");
            Console.WriteLine("2. Bogotá");
            Console.WriteLine("3. Cali");
            Console.WriteLine("4. Barranquilla");
            Console.WriteLine("5. Cartagena");
            // ... (puedes agregar más ciudades según necesites)

            int ciudadId;
            do
            {
                Console.Write("\nSeleccione la ciudad (1-20): ");
                if (!int.TryParse(Console.ReadLine(), out ciudadId) || ciudadId < 1 || ciudadId > 20)
                {
                    Console.WriteLine("Error: Por favor seleccione una ciudad válida entre 1 y 20.");
                }
            } while (ciudadId < 1 || ciudadId > 20);

            // Validar y obtener descuento
            double descuento;
            do
            {
                Console.Write("Ingrese el descuento (0-100): ");
                if (!double.TryParse(Console.ReadLine(), out descuento) || descuento < 0 || descuento > 100)
                {
                    Console.WriteLine("Error: El descuento debe ser un número entre 0 y 100.");
                }
            } while (descuento < 0 || descuento > 100);

            var proveedor = new Proveedor
            {
                Nombre = nombre,
                Apellidos = apellidos,
                Email = email,
                TipoDocumentoId = tipoDocumentoId,
                TipoTerceroId = tipoProveedorId,
                CiudadId = ciudadId,
                FechaIngreso = DateTime.Now,
                Descuento = descuento
            };

            await _repository.AddAsync(proveedor);
            Console.WriteLine("\nProveedor registrado exitosamente.");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> EmailExistsAsync(string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new MySqlCommand("SELECT COUNT(*) FROM Terceros WHERE email = @email", conn);
            cmd.Parameters.AddWithValue("@email", email);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
        }

        public async Task ListarProveedores()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA DE PROVEEDORES ===");

            try
            {
                var proveedores = await _repository.GetAllAsync();
                if (!proveedores.Any())
                {
                    Console.WriteLine("No hay proveedores registrados.");
                }
                else
                {
                    foreach (var p in proveedores)
                    {
                        Console.WriteLine($"\nID: {p.Id}");
                        Console.WriteLine($"Nombre: {p.Nombre} {p.Apellidos}");
                        Console.WriteLine($"Email: {p.Email}");
                        Console.WriteLine($"Fecha de Ingreso: {p.FechaIngreso:yyyy-MM-dd}");
                        Console.WriteLine($"Descuento: {p.Descuento}%");
                        Console.WriteLine("------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al listar proveedores: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public async Task ActualizarProveedor()
        {
            Console.Clear();
            Console.WriteLine("=== ACTUALIZAR PROVEEDOR ===");

            Console.Write("Ingrese el ID del proveedor a actualizar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ ID inválido.");
                return;
            }

            var proveedor = await _repository.GetByIdAsync(id);
            if (proveedor == null)
            {
                Console.WriteLine("❌ Proveedor no encontrado.");
                return;
            }

            Console.WriteLine("\nDatos actuales:");
            Console.WriteLine($"Nombre: {proveedor.Nombre}");
            Console.WriteLine($"Apellidos: {proveedor.Apellidos}");
            Console.WriteLine($"Email: {proveedor.Email}");
            Console.WriteLine($"Fecha de Ingreso: {proveedor.FechaIngreso:yyyy-MM-dd}");
            Console.WriteLine($"Descuento: {proveedor.Descuento}%");

            Console.WriteLine("\nIngrese los nuevos datos (deje en blanco para mantener el valor actual):");

            Console.Write("Nombre: ");
            var nombre = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                proveedor.Nombre = nombre;
            }

            Console.Write("Apellidos: ");
            var apellidos = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(apellidos))
            {
                proveedor.Apellidos = apellidos;
            }

            Console.Write("Email: ");
            var email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email))
            {
                proveedor.Email = email;
            }

            Console.Write("Fecha de Ingreso (yyyy-MM-dd): ");
            var fechaStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(fechaStr) && DateTime.TryParse(fechaStr, out DateTime fechaIngreso))
            {
                proveedor.FechaIngreso = fechaIngreso;
            }

            Console.Write("Descuento (%): ");
            var descuentoStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(descuentoStr) && double.TryParse(descuentoStr, out double descuento))
            {
                proveedor.Descuento = descuento;
            }

            // Mostrar y seleccionar tipo de documento
            var tiposDocumento = await ObtenerTiposDocumentoAsync();
            Console.WriteLine("\nTipos de Documento disponibles:");
            foreach (var tipo in tiposDocumento)
            {
                Console.WriteLine($"{tipo.Id}. {tipo.Descripcion}");
            }
            Console.Write("\nSeleccione el tipo de documento (deje en blanco para mantener el actual): ");
            var tipoDocInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(tipoDocInput) && 
                int.TryParse(tipoDocInput, out int tipoDocId) && 
                tiposDocumento.Any(t => t.Id == tipoDocId))
            {
                proveedor.TipoDocumentoId = tipoDocId;
            }

            // Mostrar y seleccionar ciudad
            var ciudades = await ObtenerCiudadesAsync();
            Console.WriteLine("\nCiudades disponibles:");
            foreach (var ciudad in ciudades)
            {
                Console.WriteLine($"{ciudad.Id}. {ciudad.Nombre}");
            }
            Console.Write("\nSeleccione la ciudad (deje en blanco para mantener la actual): ");
            var ciudadInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ciudadInput) && 
                int.TryParse(ciudadInput, out int ciudadId) && 
                ciudades.Any(c => c.Id == ciudadId))
            {
                proveedor.CiudadId = ciudadId;
            }

            try
            {
                await _repository.UpdateAsync(proveedor);
                Console.WriteLine("\n✅ Proveedor actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al actualizar proveedor: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public async Task EliminarProveedor()
        {
            Console.Clear();
            Console.WriteLine("=== ELIMINAR PROVEEDOR ===");

            Console.Write("Ingrese el ID del proveedor a eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ ID inválido.");
                return;
            }

            try
            {
                await _repository.DeleteAsync(id);
                Console.WriteLine("\n✅ Proveedor eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al eliminar proveedor: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
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
                var productos = await _repository.GetProductosAsync(id);
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