using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.application.services
{
    public class PedidoService
    {
        private readonly IPedidoRepository _repo;

        public PedidoService(IPedidoRepository repo)
        {
            _repo = repo;
        }

        public async Task MostrarTodos()
        {
            var lista = await _repo.GetAllAsync();
            foreach (var p in lista)
            {
                Console.WriteLine($"ID: {p.Id}, Cliente: {p.ClienteId}, Estado: {p.Estado}, Total: {p.Total}");
            }
        }

        public async Task MostrarDetalles(int id)
        {
            var pedido = await _repo.GetByIdAsync(id);
            if (pedido != null)
            {
                Console.WriteLine($"\nDetalles del Pedido #{id}");
                Console.WriteLine($"Cliente: {pedido.ClienteId}");
                Console.WriteLine($"Estado: {pedido.Estado}");
                Console.WriteLine($"Total: {pedido.Total}");
                Console.WriteLine("\nProductos:");
                foreach (var detalle in pedido.Detalles)
                {
                    Console.WriteLine($"- {detalle.ProductoId}: {detalle.Cantidad} x ${detalle.PrecioUnitario} = ${detalle.Subtotal}");
                }
            }
            else
            {
                Console.WriteLine("❌ Pedido no encontrado.");
            }
        }

        public async Task CrearPedido(Pedido pedido, List<DetallePedido> detalles)
        {
            pedido.Detalles = detalles;
            pedido.Total = detalles.Sum(d => d.Subtotal);
            await _repo.AddAsync(pedido);
            Console.WriteLine("✅ Pedido creado exitosamente.");
        }

        public async Task CancelarPedido(int id)
        {
            var pedido = await _repo.GetByIdAsync(id);
            if (pedido != null)
            {
                pedido.Estado = "Cancelado";
                await _repo.UpdateAsync(pedido);
                Console.WriteLine("✅ Pedido cancelado exitosamente.");
            }
            else
            {
                Console.WriteLine("❌ Pedido no encontrado.");
            }
        }
    }
} 