using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.application.services
{
    public class ClienteService
    {
        private readonly IClienteRepository _repo;

        public ClienteService(IClienteRepository repo)
        {
            _repo = repo;
        }

        public async Task MostrarTodos()
        {
            Console.Clear();
            var lista = await _repo.GetAllAsync();
            foreach (var c in lista)
            {
                Console.WriteLine($"ID: {c.Id}");
                Console.WriteLine($"Nombre: {c.Nombre}");
                Console.WriteLine($"Dirección: {c.Direccion}");
                Console.WriteLine($"Ciudad: {c.Ciudad}");
                Console.WriteLine($"Teléfono: {c.Telefono}");
                Console.WriteLine($"Correo: {c.Correo}");
                Console.WriteLine("------------------------");
            }
        }

        public async Task CrearCliente(string nombre)
        {
            Console.Clear();
            var cliente = new Cliente { Nombre = nombre };
            
            Console.Write("Dirección: ");
            cliente.Direccion = Console.ReadLine() ?? "";
            
            Console.Write("Ciudad: ");
            cliente.Ciudad = Console.ReadLine() ?? "";
            
            Console.Write("Teléfono: ");
            cliente.Telefono = Console.ReadLine() ?? "";
            
            Console.Write("Correo: ");
            cliente.Correo = Console.ReadLine() ?? "";

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
            
            Console.Write("Dirección: ");
            cliente.Direccion = Console.ReadLine() ?? cliente.Direccion;
            
            Console.Write("Ciudad: ");
            cliente.Ciudad = Console.ReadLine() ?? cliente.Ciudad;
            
            Console.Write("Teléfono: ");
            cliente.Telefono = Console.ReadLine() ?? cliente.Telefono;
            
            Console.Write("Correo: ");
            cliente.Correo = Console.ReadLine() ?? cliente.Correo;

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
    }
} 