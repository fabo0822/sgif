using sgif.domain.entities;
using sgif.domain.ports;
using MySql.Data.MySqlClient;

namespace sgif.application.services
{
    public class EmpleadoService
    {
        private readonly IEmpleadoRepository _repository;
        private readonly string _connectionString;

        public EmpleadoService(IEmpleadoRepository repository, string connectionString)
        {
            _repository = repository;
            _connectionString = connectionString;
        }

        public async Task MostrarTodos()
        {
            Console.Clear();
            var empleados = await _repository.GetAllAsync();
            Console.WriteLine("\n=== LISTA DE EMPLEADOS ===");
            foreach (var empleado in empleados)
            {
                Console.WriteLine($"ID: {empleado.Id}");
                Console.WriteLine($"Nombre: {empleado.Nombre} {empleado.Apellidos}");
                Console.WriteLine($"Email: {empleado.Email}");
                Console.WriteLine($"Salario Base: ${empleado.SalarioBase:N2}");
                Console.WriteLine($"Fecha Ingreso: {empleado.FechaIngreso:dd/MM/yyyy}");
                Console.WriteLine("------------------------");
            }
        }

        public async Task RegistrarEmpleado()
        {
            Console.WriteLine("\n=== Registro de Empleado ===");

            // Validar y obtener nombre
            string nombre;
            do
            {
                Console.Write("Ingrese el nombre del empleado: ");
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
                Console.Write("Ingrese los apellidos del empleado: ");
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
                Console.Write("Ingrese el email del empleado: ");
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

            // Validar y obtener salario base
            decimal salarioBase;
            do
            {
                Console.Write("Ingrese el salario base: ");
                if (!decimal.TryParse(Console.ReadLine(), out salarioBase) || salarioBase <= 0)
                {
                    Console.WriteLine("Error: El salario debe ser un número positivo.");
                }
            } while (salarioBase <= 0);

            // Mostrar y validar EPS
            Console.WriteLine("\nEPS disponibles:");
            Console.WriteLine("1. Sura");
            Console.WriteLine("2. Nueva EPS");
            Console.WriteLine("3. Sanitas");
            Console.WriteLine("4. Coomeva");
            Console.WriteLine("5. Compensar");

            int epsId;
            do
            {
                Console.Write("\nSeleccione la EPS (1-5): ");
                if (!int.TryParse(Console.ReadLine(), out epsId) || epsId < 1 || epsId > 5)
                {
                    Console.WriteLine("Error: Por favor seleccione una EPS válida entre 1 y 5.");
                }
            } while (epsId < 1 || epsId > 5);

            // Mostrar y validar ARL
            Console.WriteLine("\nARL disponibles:");
            Console.WriteLine("1. Sura");
            Console.WriteLine("2. Colmena");
            Console.WriteLine("3. Positiva");
            Console.WriteLine("4. La Equidad");
            Console.WriteLine("5. Seguros Bolívar");

            int arlId;
            do
            {
                Console.Write("\nSeleccione la ARL (1-5): ");
                if (!int.TryParse(Console.ReadLine(), out arlId) || arlId < 1 || arlId > 5)
                {
                    Console.WriteLine("Error: Por favor seleccione una ARL válida entre 1 y 5.");
                }
            } while (arlId < 1 || arlId > 5);

            var empleado = new Empleado
            {
                Nombre = nombre,
                Apellidos = apellidos,
                Email = email,
                TipoDocumentoId = tipoDocumentoId,
                CiudadId = ciudadId,
                FechaIngreso = DateTime.Now,
                SalarioBase = salarioBase,
                EpsId = epsId,
                ArlId = arlId
            };

            await _repository.AddAsync(empleado);
            Console.WriteLine("\nEmpleado registrado exitosamente.");
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

        public async Task ActualizarEmpleado()
        {
            Console.WriteLine("\n=== ACTUALIZAR EMPLEADO ===");
            Console.Write("ID del empleado: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ ID inválido.");
                return;
            }

            var empleado = await _repository.GetByIdAsync(id);
            if (empleado == null)
            {
                Console.WriteLine("❌ Empleado no encontrado.");
                return;
            }

            Console.Write("Nuevo nombre: ");
            empleado.Nombre = Console.ReadLine() ?? empleado.Nombre;

            Console.Write("Nuevos apellidos: ");
            empleado.Apellidos = Console.ReadLine() ?? empleado.Apellidos;

            Console.Write("Nuevo email: ");
            empleado.Email = Console.ReadLine() ?? empleado.Email;

            Console.Write("Nuevo salario base: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal nuevoSalario))
            {
                empleado.SalarioBase = nuevoSalario;
            }

            await _repository.UpdateAsync(empleado);
            Console.WriteLine("✅ Empleado actualizado exitosamente.");
        }

        public async Task EliminarEmpleado()
        {
            Console.WriteLine("\n=== ELIMINAR EMPLEADO ===");
            Console.Write("ID del empleado: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ ID inválido.");
                return;
            }

            await _repository.DeleteAsync(id);
            Console.WriteLine("✅ Empleado eliminado exitosamente.");
        }

        public async Task VerDetalles()
        {
            Console.WriteLine("\n=== VER DETALLES DE EMPLEADO ===");
            Console.Write("ID del empleado: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ ID inválido.");
                return;
            }

            var empleado = await _repository.GetByIdAsync(id);
            if (empleado == null)
            {
                Console.WriteLine("❌ Empleado no encontrado.");
                return;
            }

            Console.WriteLine("\n=== DETALLES DEL EMPLEADO ===");
            Console.WriteLine($"ID: {empleado.Id}");
            Console.WriteLine($"Nombre: {empleado.Nombre} {empleado.Apellidos}");
            Console.WriteLine($"Email: {empleado.Email}");
            Console.WriteLine($"Salario Base: ${empleado.SalarioBase:N2}");
            Console.WriteLine($"Fecha Ingreso: {empleado.FechaIngreso:dd/MM/yyyy}");
            Console.WriteLine($"EPS ID: {empleado.EpsId}");
            Console.WriteLine($"ARL ID: {empleado.ArlId}");
        }
    }
} 