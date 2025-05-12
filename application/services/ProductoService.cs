using MySql.Data.MySqlClient;
using sgif.domain.entities;
using sgif.domain.ports;
using System;

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
            try
            {
                var productos = await _repository.GetAllAsync();
                if (!productos.Any())
                {
                    Console.WriteLine("\nNo hay productos registrados.");
                    return;
                }

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
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al mostrar productos: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }

        public async Task RegistrarProducto()
        {
            try
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
                Console.WriteLine("\n✅ Producto registrado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al registrar producto: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }

        private async Task<bool> BarcodeExistsAsync(string barcode)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM Producto WHERE barcode = @barcode", conn);
                cmd.Parameters.AddWithValue("@barcode", barcode);
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al verificar código de barras: {ex.Message}");
                throw;
            }
        }

        public async Task ActualizarProducto()
        {
            try
            {
                Console.WriteLine("\n=== ACTUALIZAR PRODUCTO ===");
                Console.Write("ID del producto: ");
                var id = Console.ReadLine() ?? string.Empty;

                var producto = await _repository.GetById(id);
                if (producto == null)
                {
                    Console.WriteLine("❌ Producto no encontrado.");
                    return;
                }

                Console.Write("Nuevo nombre (Enter para mantener el actual): ");
                var nuevoNombre = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(nuevoNombre))
                {
                    producto.Nombre = nuevoNombre;
                }

                Console.Write("Nuevo stock (Enter para mantener el actual): ");
                var stockInput = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(stockInput) && int.TryParse(stockInput, out int nuevoStock) && nuevoStock >= 0)
                {
                    producto.Stock = nuevoStock;
                }

                Console.Write("Nuevo stock mínimo (Enter para mantener el actual): ");
                var stockMinInput = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(stockMinInput) && int.TryParse(stockMinInput, out int nuevoStockMin) && nuevoStockMin >= 0)
                {
                    producto.StockMin = nuevoStockMin;
                }

                Console.Write("Nuevo stock máximo (Enter para mantener el actual): ");
                var stockMaxInput = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(stockMaxInput) && int.TryParse(stockMaxInput, out int nuevoStockMax) && nuevoStockMax > producto.StockMin)
                {
                    producto.StockMax = nuevoStockMax;
                }

                producto.UpdatedAt = DateTime.Now;

                await _repository.Update(producto);
                Console.WriteLine("✅ Producto actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al actualizar producto: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }

        public async Task EliminarProducto()
        {
            try
            {
                Console.WriteLine("\n=== ELIMINAR PRODUCTO ===");
                Console.Write("ID del producto: ");
                var id = Console.ReadLine() ?? string.Empty;

                var producto = await _repository.GetById(id);
                if (producto == null)
                {
                    Console.WriteLine("❌ Producto no encontrado.");
                    return;
                }

                Console.Write($"¿Está seguro que desea eliminar el producto '{producto.Nombre}'? (S/N): ");
                if (Console.ReadLine()?.ToUpper() != "S")
                {
                    Console.WriteLine("Operación cancelada.");
                    return;
                }

                await _repository.DeleteAsync(id);
                Console.WriteLine("✅ Producto eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al eliminar producto: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }

        public async Task VerDetalles()
        {
            try
            {
                Console.WriteLine("\n=== VER DETALLES DE PRODUCTO ===");
                Console.Write("ID del producto: ");
                var id = Console.ReadLine() ?? string.Empty;

                var producto = await _repository.GetById(id);
                if (producto == null)
                {
                    Console.WriteLine("❌ Producto no encontrado.");
                    return;
                }

                Console.WriteLine("\n=== DETALLES DEL PRODUCTO ===");
                Console.WriteLine($"ID: {producto.Id}");
                Console.WriteLine($"Nombre: {producto.Nombre}");
                Console.WriteLine($"Código de Barras: {producto.Barcode}");
                Console.WriteLine($"Stock: {producto.Stock}");
                Console.WriteLine($"Stock Mínimo: {producto.StockMin}");
                Console.WriteLine($"Stock Máximo: {producto.StockMax}");
                Console.WriteLine($"Fecha Creación: {producto.CreatedAt:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Última Actualización: {producto.UpdatedAt:dd/MM/yyyy HH:mm}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al ver detalles del producto: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
    }
}