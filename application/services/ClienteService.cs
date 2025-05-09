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
            var lista = await _repo.GetAllAsync();
            foreach (var c in lista)
            {
                Console.WriteLine($"ID: {c.Id}, Nombre: {c.Nombre}");
            }
        }

        public async Task CrearCliente(string nombre)
        {
            await _repo.AddAsync(new Cliente { Nombre = nombre });
        }

        public async Task ActualizarCliente(int id, string nuevoNombre)
        {
            await _repo.UpdateAsync(new Cliente { Id = id, Nombre = nuevoNombre });
        }

        public async Task EliminarCliente(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
} 