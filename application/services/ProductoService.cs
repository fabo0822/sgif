using MySql.Data.MySqlClient;
using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.application.services
{
    public class ProductoService
    {
        private readonly string _connectionString;
        private readonly IProductoRepository _repository;

        public ProductoService(string connectionString, IProductoRepository repository)
        {
            _connectionString = connectionString;
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
            Console.WriteLine("\n=== Registro de Producto ===");

            // Validar y obtener nombre
            string nombre;
            do
            {
                Console.Write("Ingrese el nombre del producto: ");
                nombre = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.WriteLine("Error: El nombre no puede estar vacío.");
                }
            } while (string.IsNullOrWhiteSpace(nombre));

            // Validar y obtener código de barras
            string barcode;
            bool barcodeValido = false;
            do
            {
                Console.Write("Ingrese el código de barras: ");
                barcode = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(barcode))
                {
                    Console.WriteLine("Error: El código de barras no puede estar vacío.");
                    continue;
                }
                if (await BarcodeExistsAsync(barcode))
                {
                    Console.WriteLine("Error: Este código de barras ya está registrado.");
                    continue;
                }
                barcodeValido = true;
            } while (!barcodeValido);

            // Validar y obtener stock inicial
            int stock;
            do
            {
                Console.Write("Ingrese el stock inicial: ");
                if (!int.TryParse(Console.ReadLine(), out stock) || stock < 0)
                {
                    Console.WriteLine("Error: El stock debe ser un número positivo o cero.");
                }
            } while (stock < 0);

            // Validar y obtener stock mínimo
            int stockMin;
            do
            {
                Console.Write("Ingrese el stock mínimo: ");
                if (!int.TryParse(Console.ReadLine(), out stockMin) || stockMin < 0)
                {
                    Console.WriteLine("Error: El stock mínimo debe ser un número positivo o cero.");
                }
            } while (stockMin < 0);

            // Validar y obtener stock máximo
            int stockMax;
            do
            {
                Console.Write("Ingrese el stock máximo: ");
                if (!int.TryParse(Console.ReadLine(), out stockMax) || stockMax <= stockMin)
                {
                    Console.WriteLine("Error: El stock máximo debe ser mayor que el stock mínimo.");
                }
            } while (stockMax <= stockMin);

            var producto = new Producto
            {
                Id = Guid.NewGuid().ToString("N").Substring(0, 20),
                Nombre = nombre,
                Barcode = barcode,
                Stock = stock,
                StockMin = stockMin,
                StockMax = stockMax,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repository.AddAsync(producto);
            Console.WriteLine("\nProducto registrado exitosamente.");
        }

        private async Task<bool> BarcodeExistsAsync(string barcode)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new MySqlCommand("SELECT COUNT(*) FROM Productos WHERE barcode = @barcode", conn);
            cmd.Parameters.AddWithValue("@barcode", barcode);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
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