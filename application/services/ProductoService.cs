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
            Console.WriteLine("\n=== LISTA DE PRODUCTOS ===");
            foreach (var producto in productos)
            {
                Console.WriteLine($"ID: {producto.Id}");
                Console.WriteLine($"Nombre: {producto.Nombre}");
                Console.WriteLine($"Stock: {producto.Stock}");
                Console.WriteLine($"Stock Mínimo: {producto.StockMin}");
                Console.WriteLine($"Stock Máximo: {producto.StockMax}");
                Console.WriteLine("------------------------");
            }
        }

        public async Task RegistrarProducto()
        {

            Console.Clear();
            Console.WriteLine("\n=== REGISTRAR NUEVO PRODUCTO ===");
            var producto = new Producto();

            Console.Write("Nombre: ");
            producto.Nombre = Console.ReadLine() ?? string.Empty;

            Console.Write("Stock: ");
            if (!int.TryParse(Console.ReadLine(), out int stock))
            {
                Console.WriteLine("❌ Stock inválido.");
                return;
            }
            producto.Stock = stock;

            Console.Write("Stock Mínimo: ");
            if (!int.TryParse(Console.ReadLine(), out int stockMin))
            {
                Console.WriteLine("❌ Stock mínimo inválido.");
                return;
            }
            producto.StockMin = stockMin;

            Console.Write("Stock Máximo: ");
            if (!int.TryParse(Console.ReadLine(), out int stockMax))
            {
                Console.WriteLine("❌ Stock máximo inválido.");
                return;
            }
            producto.StockMax = stockMax;

            producto.Id = Guid.NewGuid().ToString("N").Substring(0, 20);
            producto.CreatedAt = DateTime.Now;
            producto.UpdatedAt = DateTime.Now;

            await _repository.AddAsync(producto);
            Console.WriteLine("✅ Producto registrado exitosamente.");
        }

        public async Task ActualizarProducto()
        {
            Console.WriteLine("\n=== ACTUALIZAR PRODUCTO ===");
            Console.Write("ID del producto: ");
            var id = Console.ReadLine() ?? string.Empty;

            var producto = await _repository.GetByIdAsync(id);
            if (producto == null)
            {
                Console.WriteLine("❌ Producto no encontrado.");
                return;
            }

            Console.Write("Nuevo nombre: ");
            producto.Nombre = Console.ReadLine() ?? producto.Nombre;

            Console.Write("Nuevo stock: ");
            if (int.TryParse(Console.ReadLine(), out int nuevoStock))
            {
                producto.Stock = nuevoStock;
            }

            Console.Write("Nuevo stock mínimo: ");
            if (int.TryParse(Console.ReadLine(), out int nuevoStockMin))
            {
                producto.StockMin = nuevoStockMin;
            }

            Console.Write("Nuevo stock máximo: ");
            if (int.TryParse(Console.ReadLine(), out int nuevoStockMax))
            {
                producto.StockMax = nuevoStockMax;
            }

            producto.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(producto);
            Console.WriteLine("✅ Producto actualizado exitosamente.");
        }

        public async Task EliminarProducto()
        {
            Console.WriteLine("\n=== ELIMINAR PRODUCTO ===");
            Console.Write("ID del producto: ");
            var id = Console.ReadLine() ?? string.Empty;

            await _repository.DeleteAsync(id);
            Console.WriteLine("✅ Producto eliminado exitosamente.");
        }

        public async Task VerDetalles()
        {
            Console.WriteLine("\n=== VER DETALLES DE PRODUCTO ===");
            Console.Write("ID del producto: ");
            var id = Console.ReadLine() ?? string.Empty;

            var producto = await _repository.GetByIdAsync(id);
            if (producto == null)
            {
                Console.WriteLine("❌ Producto no encontrado.");
                return;
            }

            Console.WriteLine("\n=== DETALLES DEL PRODUCTO ===");
            Console.WriteLine($"ID: {producto.Id}");
            Console.WriteLine($"Nombre: {producto.Nombre}");
            Console.WriteLine($"Stock: {producto.Stock}");
            Console.WriteLine($"Stock Mínimo: {producto.StockMin}");
            Console.WriteLine($"Stock Máximo: {producto.StockMax}");
            Console.WriteLine($"Fecha Creación: {producto.CreatedAt:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Última Actualización: {producto.UpdatedAt:dd/MM/yyyy HH:mm}");
        }
    }
} 