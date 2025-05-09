using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.application.services
{
    public class ProveedorService
    {
        private readonly IProveedorRepository _repository;

        public ProveedorService(IProveedorRepository repository)
        {
            _repository = repository;
        }

        public async Task RegistrarProveedor()
        {
            Console.Clear();
            Console.WriteLine("=== REGISTRO DE PROVEEDOR ===");

            var proveedor = new Proveedor();

            Console.Write("Nombre: ");
            proveedor.Nombre = Console.ReadLine() ?? string.Empty;

            Console.Write("Apellidos: ");
            proveedor.Apellidos = Console.ReadLine() ?? string.Empty;

            Console.Write("Email: ");
            proveedor.Email = Console.ReadLine() ?? string.Empty;

            Console.Write("Fecha de Ingreso (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime fechaIngreso))
            {
                proveedor.FechaIngreso = fechaIngreso;
            }
            else
            {
                Console.WriteLine("Fecha inválida. Se usará la fecha actual.");
                proveedor.FechaIngreso = DateTime.Now;
            }

            Console.Write("Descuento (%): ");
            if (double.TryParse(Console.ReadLine(), out double descuento))
            {
                proveedor.Descuento = descuento;
            }
            else
            {
                Console.WriteLine("Valor inválido. Se usará 0%.");
                proveedor.Descuento = 0;
            }

            Console.Write("Tipo de Documento (1-CC, 2-NIT, 3-Pasaporte): ");
            if (int.TryParse(Console.ReadLine(), out int tipoDoc))
            {
                proveedor.TipoDocumentoId = tipoDoc;
            }
            else
            {
                Console.WriteLine("Valor inválido. Se usará CC (1).");
                proveedor.TipoDocumentoId = 1;
            }

            Console.Write("Ciudad ID: ");
            if (int.TryParse(Console.ReadLine(), out int ciudadId))
            {
                proveedor.CiudadId = ciudadId;
            }
            else
            {
                Console.WriteLine("Valor inválido. Se usará 1.");
                proveedor.CiudadId = 1;
            }

            try
            {
                await _repository.AddAsync(proveedor);
                Console.WriteLine("\nProveedor registrado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al registrar proveedor: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public async Task ListarProveedores()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA DE PROVEEDORES ===");

            try
            {
                var proveedores = await _repository.GetAllAsync();
                if (!proveedores.Any())
                {
                    Console.WriteLine("No hay proveedores registrados.");
                }
                else
                {
                    foreach (var p in proveedores)
                    {
                        Console.WriteLine($"\nID: {p.Id}");
                        Console.WriteLine($"Nombre: {p.Nombre} {p.Apellidos}");
                        Console.WriteLine($"Email: {p.Email}");
                        Console.WriteLine($"Fecha de Ingreso: {p.FechaIngreso:yyyy-MM-dd}");
                        Console.WriteLine($"Descuento: {p.Descuento}%");
                        Console.WriteLine("------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al listar proveedores: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public async Task ActualizarProveedor()
        {
            Console.Clear();
            Console.WriteLine("=== ACTUALIZAR PROVEEDOR ===");

            Console.Write("Ingrese el ID del proveedor a actualizar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            var proveedor = await _repository.GetByIdAsync(id);
            if (proveedor == null)
            {
                Console.WriteLine("Proveedor no encontrado.");
                return;
            }

            Console.WriteLine("\nDatos actuales:");
            Console.WriteLine($"Nombre: {proveedor.Nombre}");
            Console.WriteLine($"Apellidos: {proveedor.Apellidos}");
            Console.WriteLine($"Email: {proveedor.Email}");
            Console.WriteLine($"Fecha de Ingreso: {proveedor.FechaIngreso:yyyy-MM-dd}");
            Console.WriteLine($"Descuento: {proveedor.Descuento}%");

            Console.WriteLine("\nIngrese los nuevos datos (deje en blanco para mantener el valor actual):");

            Console.Write("Nombre: ");
            var nombre = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                proveedor.Nombre = nombre;
            }

            Console.Write("Apellidos: ");
            var apellidos = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(apellidos))
            {
                proveedor.Apellidos = apellidos;
            }

            Console.Write("Email: ");
            var email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email))
            {
                proveedor.Email = email;
            }

            Console.Write("Fecha de Ingreso (yyyy-MM-dd): ");
            var fechaStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(fechaStr) && DateTime.TryParse(fechaStr, out DateTime fechaIngreso))
            {
                proveedor.FechaIngreso = fechaIngreso;
            }

            Console.Write("Descuento (%): ");
            var descuentoStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(descuentoStr) && double.TryParse(descuentoStr, out double descuento))
            {
                proveedor.Descuento = descuento;
            }

            try
            {
                await _repository.UpdateAsync(proveedor);
                Console.WriteLine("\nProveedor actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al actualizar proveedor: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public async Task EliminarProveedor()
        {
            Console.Clear();
            Console.WriteLine("=== ELIMINAR PROVEEDOR ===");

            Console.Write("Ingrese el ID del proveedor a eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            try
            {
                await _repository.DeleteAsync(id);
                Console.WriteLine("\nProveedor eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al eliminar proveedor: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public async Task VerProductosProveedor()
        {
            Console.Clear();
            Console.WriteLine("=== PRODUCTOS DEL PROVEEDOR ===");

            Console.Write("Ingrese el ID del proveedor: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            try
            {
                var productos = await _repository.GetProductosAsync(id);
                if (!productos.Any())
                {
                    Console.WriteLine("El proveedor no tiene productos asociados.");
                }
                else
                {
                    foreach (var p in productos)
                    {
                        Console.WriteLine($"\nID: {p.Id}");
                        Console.WriteLine($"Nombre: {p.Nombre}");
                        Console.WriteLine($"Stock: {p.Stock}");
                        Console.WriteLine($"Stock Mínimo: {p.StockMin}");
                        Console.WriteLine($"Stock Máximo: {p.StockMax}");
                        Console.WriteLine("------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al obtener productos: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
} 