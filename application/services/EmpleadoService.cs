using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.application.services
{
    public class EmpleadoService
    {
        private readonly IEmpleadoRepository _repository;

        public EmpleadoService(IEmpleadoRepository repository)
        {
            _repository = repository;
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

            Console.Write("Día de Pago (1-31): ");
            if (!int.TryParse(Console.ReadLine(), out int diaPago) || diaPago < 1 || diaPago > 31)
            {
                Console.WriteLine("❌ Día de pago inválido.");
                return;
            }

            Console.Write("ID EPS: ");
            if (!int.TryParse(Console.ReadLine(), out int epsId))
            {
                Console.WriteLine("❌ ID EPS inválido.");
                return;
            }
            empleado.EpsId = epsId;

            Console.Write("ID ARL: ");
            if (!int.TryParse(Console.ReadLine(), out int arlId))
            {
                Console.WriteLine("❌ ID ARL inválido.");
                return;
            }
            empleado.ArlId = arlId;

            empleado.FechaIngreso = DateTime.Now;

            await _repository.AddAsync(empleado);
            Console.WriteLine("✅ Empleado registrado exitosamente.");
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