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
            Console.Clear();
            Console.WriteLine("\n=== REGISTRAR NUEVO EMPLEADO ===");
            var empleado = new Empleado();

            Console.Write("Nombre: ");
            empleado.Nombre = Console.ReadLine() ?? string.Empty;

            Console.Write("Apellidos: ");
            empleado.Apellidos = Console.ReadLine() ?? string.Empty;

            Console.Write("Email: ");
            empleado.Email = Console.ReadLine() ?? string.Empty;

            Console.Write("Salario Base: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal salario))
            {
                Console.WriteLine("❌ Salario inválido.");
                return;
            }
            empleado.SalarioBase = salario;

            Console.WriteLine("\nEPS disponibles:");
            Console.WriteLine("1. Sura");
            Console.WriteLine("2. Nueva EPS");
            Console.WriteLine("3. Sanitas");
            Console.Write("\nSeleccione la EPS (1-3): ");
            if (!int.TryParse(Console.ReadLine(), out int epsId) || epsId < 1 || epsId > 3)
            {
                Console.WriteLine("❌ EPS inválida.");
                return;
            }
            empleado.EpsId = epsId;

            Console.WriteLine("\nARL disponibles:");
            Console.WriteLine("1. Sura");
            Console.WriteLine("2. Colmena");
            Console.WriteLine("3. Positiva");
            Console.Write("\nSeleccione la ARL (1-3): ");
            if (!int.TryParse(Console.ReadLine(), out int arlId) || arlId < 1 || arlId > 3)
            {
                Console.WriteLine("❌ ARL inválida.");
                return;
            }
            empleado.ArlId = arlId;

            Console.WriteLine("\nTipos de documento disponibles:");
            Console.WriteLine("1. Cédula de Ciudadanía");
            Console.WriteLine("2. Cédula de Extranjería");
            Console.WriteLine("3. Pasaporte");
            Console.WriteLine("4. Tarjeta de Identidad");
            Console.Write("\nSeleccione el tipo de documento (1-4): ");
            if (!int.TryParse(Console.ReadLine(), out int tipoDocId) || tipoDocId < 1 || tipoDocId > 4)
            {
                Console.WriteLine("❌ Tipo de documento inválido.");
                return;
            }
            empleado.TipoDocumentoId = tipoDocId;

            Console.WriteLine("\nCiudades disponibles:");
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT id, nombre FROM Ciudad ORDER BY nombre", conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"{reader.GetInt32(0)}. {reader.GetString(1)}");
                }
            }
            Console.Write("\nSeleccione la ciudad: ");
            if (!int.TryParse(Console.ReadLine(), out int ciudadId))
            {
                Console.WriteLine("❌ Ciudad inválida.");
                return;
            }
            empleado.CiudadId = ciudadId;

            bool fechaValida = false;
            while (!fechaValida)
            {
                Console.Write("\nFecha de Ingreso (yyyy-MM-dd): ");
                string fechaStr = Console.ReadLine() ?? string.Empty;
                
                if (DateTime.TryParseExact(fechaStr, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime fecha))
                {
                    empleado.FechaIngreso = fecha;
                    fechaValida = true;
                }
                else
                {
                    Console.WriteLine("❌ Formato de fecha inválido. Por favor use el formato yyyy-MM-dd (ejemplo: 2024-03-21)");
                }
            }

            try
            {
                await _repository.AddAsync(empleado);
                Console.WriteLine("✅ Empleado registrado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al registrar empleado: {ex.Message}");
            }
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