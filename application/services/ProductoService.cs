using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.application.services
{
    public class ProductoService
    {
        private readonly IProductoRepository _repository;

        public ProductoService(IProductoRepository repository)
        {
            _repository = repository;
        }

        public async Task MostrarTodos()
        {
            var productos = await _repository.GetAllAsync();
            foreach (var producto in productos)
            {
                Console.WriteLine($"ID: {producto.Id}, Nombre: {producto.Nombre}, Stock: {producto.Stock}, Precio: {producto.Precio:C}");
            }
        }

        public async Task CrearProducto(Producto producto)
        {
            await _repository.AddAsync(producto);
            Console.WriteLine("✅ Producto creado exitosamente.");
        }

        public async Task ActualizarProducto(int id, string nombre, int stock, decimal precio)
        {
            var producto = await _repository.GetByIdAsync(id);
            if (producto != null)
            {
                producto.Nombre = nombre;
                producto.Stock = stock;
                producto.Precio = precio;
                await _repository.UpdateAsync(producto);
                Console.WriteLine("✅ Producto actualizado exitosamente.");
            }
            else
            {
                Console.WriteLine("❌ Producto no encontrado.");
            }
        }

        public async Task EliminarProducto(int id)
        {
            await _repository.DeleteAsync(id);
            Console.WriteLine("✅ Producto eliminado exitosamente.");
        }

        public async Task<Producto?> ObtenerProducto(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
} 