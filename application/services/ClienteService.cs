using sgif.domain.entities;
using sgif.domain.ports;
using MySql.Data.MySqlClient;

namespace sgif.application.services
{
    public class ClienteService
    {
        private readonly IClienteRepository _repo;
        private readonly string _connectionString;

        public ClienteService(IClienteRepository repo, string connectionString)
        {
            _repo = repo;
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

        public async Task MostrarTodos()
        {
            Console.Clear();
            var lista = await _repo.GetAllAsync();
            foreach (var c in lista)
            {
                Console.WriteLine($"ID: {c.Id}");
                Console.WriteLine($"Nombre: {c.Nombre} {c.Apellidos}");
                Console.WriteLine($"Email: {c.Email}");
                Console.WriteLine($"Fecha de Nacimiento: {c.FechaNacimiento:dd/MM/yyyy}");
                Console.WriteLine($"Fecha de Compra: {c.FechaCompra:dd/MM/yyyy}");
                Console.WriteLine("------------------------");
            }
        }

        public async Task CrearCliente(string nombre)
        {
            Console.Clear();
            var cliente = new Cliente 
            { 
                Nombre = nombre,
                FechaCompra = DateTime.Now
            };
            
            Console.Write("Apellidos: ");
            cliente.Apellidos = Console.ReadLine() ?? "";
            
            Console.Write("Email: ");
            cliente.Email = Console.ReadLine() ?? "";
            
            Console.Write("Fecha de Nacimiento (dd/MM/yyyy): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime fechaNac))
            {
                cliente.FechaNacimiento = fechaNac;
            }

            // Mostrar y seleccionar tipo de documento
            var tiposDocumento = await ObtenerTiposDocumentoAsync();
            Console.WriteLine("\nTipos de Documento disponibles:");
            foreach (var tipo in tiposDocumento)
            {
                Console.WriteLine($"{tipo.Id}. {tipo.Descripcion}");
            }
            Console.Write("\nSeleccione el tipo de documento: ");
            if (int.TryParse(Console.ReadLine(), out int tipoDocId) && tiposDocumento.Any(t => t.Id == tipoDocId))
            {
                cliente.TipoDocumentoId = tipoDocId;
            }
            else
            {
                Console.WriteLine("❌ Tipo de documento inválido.");
                return;
            }

            // Mostrar y seleccionar tipo de tercero
            var tiposTercero = await ObtenerTiposTerceroAsync();
            Console.WriteLine("\nTipos de Tercero disponibles:");
            foreach (var tipo in tiposTercero)
            {
                Console.WriteLine($"{tipo.Id}. {tipo.Descripcion}");
            }
            Console.Write("\nSeleccione el tipo de tercero: ");
            if (int.TryParse(Console.ReadLine(), out int tipoTerceroId) && tiposTercero.Any(t => t.Id == tipoTerceroId))
            {
                cliente.TipoTerceroId = tipoTerceroId;
            }
            else
            {
                Console.WriteLine("❌ Tipo de tercero inválido.");
                return;
            }

            // Mostrar y seleccionar ciudad
            var ciudades = await ObtenerCiudadesAsync();
            Console.WriteLine("\nCiudades disponibles:");
            foreach (var ciudad in ciudades)
            {
                Console.WriteLine($"{ciudad.Id}. {ciudad.Nombre}");
            }
            Console.Write("\nSeleccione la ciudad: ");
            if (int.TryParse(Console.ReadLine(), out int ciudadId) && ciudades.Any(c => c.Id == ciudadId))
            {
                cliente.CiudadId = ciudadId;
            }
            else
            {
                Console.WriteLine("❌ Ciudad inválida.");
                return;
            }

            // Generar un ID único para el tercero
            cliente.TerceroId = Guid.NewGuid().ToString("N").Substring(0, 20);

            await _repo.AddAsync(cliente);
            Console.WriteLine("✅ Cliente creado exitosamente.");
        }

        public async Task ActualizarCliente(int id, string nuevoNombre)
        {
            Console.Clear();
            var cliente = await _repo.GetByIdAsync(id);
            if (cliente == null)
            {
                Console.WriteLine("❌ Cliente no encontrado.");
                return;
            }

            cliente.Nombre = nuevoNombre;
            
            Console.Write("Apellidos: ");
            cliente.Apellidos = Console.ReadLine() ?? cliente.Apellidos;
            
            Console.Write("Email: ");
            cliente.Email = Console.ReadLine() ?? cliente.Email;
            
            Console.Write("Fecha de Nacimiento (dd/MM/yyyy): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime fechaNac))
            {
                cliente.FechaNacimiento = fechaNac;
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
                cliente.TipoDocumentoId = tipoDocId;
            }

            // Mostrar y seleccionar tipo de tercero
            var tiposTercero = await ObtenerTiposTerceroAsync();
            Console.WriteLine("\nTipos de Tercero disponibles:");
            foreach (var tipo in tiposTercero)
            {
                Console.WriteLine($"{tipo.Id}. {tipo.Descripcion}");
            }
            Console.Write("\nSeleccione el tipo de tercero (deje en blanco para mantener el actual): ");
            var tipoTerceroInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(tipoTerceroInput) && 
                int.TryParse(tipoTerceroInput, out int tipoTerceroId) && 
                tiposTercero.Any(t => t.Id == tipoTerceroId))
            {
                cliente.TipoTerceroId = tipoTerceroId;
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
                cliente.CiudadId = ciudadId;
            }

            await _repo.UpdateAsync(cliente);
            Console.WriteLine("✅ Cliente actualizado exitosamente.");
        }

        public async Task EliminarCliente(int id)
        {
            Console.Clear();
            var cliente = await _repo.GetByIdAsync(id);
            if (cliente == null)
            {
                Console.WriteLine("❌ Cliente no encontrado.");
                return;
            }

            await _repo.DeleteAsync(id);
            Console.WriteLine("✅ Cliente eliminado exitosamente.");
        }

        public async Task RegistrarCliente()
        {
            Console.WriteLine("\n=== Registro de Cliente ===");

            // Validar y obtener nombre
            string nombre;
            do
            {
                Console.Write("Ingrese el nombre del cliente: ");
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
                Console.Write("Ingrese los apellidos del cliente: ");
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
                Console.Write("Ingrese el email del cliente: ");
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

            int tipoDocumentoId;
            do
            {
                Console.Write("\nSeleccione el tipo de documento (1-4): ");
                if (!int.TryParse(Console.ReadLine(), out tipoDocumentoId) || tipoDocumentoId < 1 || tipoDocumentoId > 4)
                {
                    Console.WriteLine("Error: Por favor seleccione un número válido entre 1 y 4.");
                }
            } while (tipoDocumentoId < 1 || tipoDocumentoId > 4);

            // Mostrar y validar tipo de cliente
            Console.WriteLine("\nTipos de cliente disponibles:");
            Console.WriteLine("1. Cliente");
            Console.WriteLine("4. Cliente VIP");
            Console.WriteLine("5. Cliente Corporativo");
            Console.WriteLine("10. Cliente Mayorista");
            Console.WriteLine("11. Cliente Minorista");
            Console.WriteLine("16. Cliente Frecuente");
            Console.WriteLine("17. Cliente Nuevo");
            Console.WriteLine("20. Cliente Especial");

            int tipoClienteId;
            do
            {
                Console.Write("\nSeleccione el tipo de cliente (1,4,5,10,11,16,17,20): ");
                if (!int.TryParse(Console.ReadLine(), out tipoClienteId) || 
                    !new[] { 1, 4, 5, 10, 11, 16, 17, 20 }.Contains(tipoClienteId))
                {
                    Console.WriteLine("Error: Por favor seleccione un tipo de cliente válido.");
                }
            } while (!new[] { 1, 4, 5, 10, 11, 16, 17, 20 }.Contains(tipoClienteId));

            // Mostrar y validar ciudad
            Console.WriteLine("\nCiudades disponibles:");
            Console.WriteLine("1. Medellín");
            Console.WriteLine("2. Bogotá");
            Console.WriteLine("3. Cali");
            Console.WriteLine("4. Barranquilla");
            Console.WriteLine("5. Cartagena");
            Console.WriteLine("6. Bucaramanga");
            Console.WriteLine("7. Pereira");
            Console.WriteLine("8. Santa Marta");
            Console.WriteLine("9. Ibagué");
            Console.WriteLine("10. Cúcuta");
            Console.WriteLine("11. Pasto");
            Console.WriteLine("12. Manizales");
            Console.WriteLine("13. Neiva");
            Console.WriteLine("14. Villavicencio");
            Console.WriteLine("15. Montería");
            Console.WriteLine("16. Valledupar");
            Console.WriteLine("17. Armenia");
            Console.WriteLine("18. Sincelejo");
            Console.WriteLine("19. Popayán");
            Console.WriteLine("20. Tunja");

            int ciudadId;
            do
            {
                Console.Write("\nSeleccione la ciudad (1-20): ");
                if (!int.TryParse(Console.ReadLine(), out ciudadId) || ciudadId < 1 || ciudadId > 20)
                {
                    Console.WriteLine("Error: Por favor seleccione una ciudad válida entre 1 y 20.");
                }
            } while (ciudadId < 1 || ciudadId > 20);

            // Validar y obtener fecha de nacimiento
            DateTime fechaNacimiento;
            do
            {
                Console.Write("Ingrese la fecha de nacimiento (yyyy-MM-dd): ");
                if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, 
                    System.Globalization.DateTimeStyles.None, out fechaNacimiento))
                {
                    Console.WriteLine("Error: Formato de fecha inválido. Use yyyy-MM-dd (ejemplo: 1990-01-01)");
                }
            } while (fechaNacimiento == default);

            var cliente = new Cliente
            {
                Nombre = nombre,
                Apellidos = apellidos,
                Email = email,
                TipoDocumentoId = tipoDocumentoId,
                TipoTerceroId = tipoClienteId,
                CiudadId = ciudadId,
                FechaNacimiento = fechaNacimiento,
                FechaCompra = DateTime.Now
            };

            await _repo.AddAsync(cliente);
            Console.WriteLine("\nCliente registrado exitosamente.");
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
    }
} 