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
                Console.WriteLine($"ID: {c.Id}, Nombre: {c.Nombre}");
            }
        }

        public async Task CrearCliente(string nombre)
        {
            Console.Clear();
            await _repo.AddAsync(new Cliente { Nombre = nombre });
        }

        public async Task ActualizarCliente(int id, string nuevoNombre)
        {
            Console.Clear();
            await _repo.UpdateAsync(new Cliente { Id = id, Nombre = nuevoNombre });
        }

        public async Task EliminarCliente(int id)
        {
            Console.Clear();
            await _repo.DeleteAsync(id);
        }
    }
} 