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

            bool fechaValida = false;
            while (!fechaValida)
            {
                Console.Write("\nFecha de Ingreso (yyyy-MM-dd): ");
                string fechaStr = Console.ReadLine() ?? string.Empty;
                
                if (DateTime.TryParseExact(fechaStr, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime fecha))
                {
                    proveedor.FechaIngreso = fecha;
                    fechaValida = true;
                }
                else
                {
                    Console.WriteLine("❌ Formato de fecha inválido. Por favor use el formato yyyy-MM-dd (ejemplo: 2024-03-21)");
                }
            }

            bool descuentoValido = false;
            while (!descuentoValido)
            {
                Console.Write("\nDescuento (%): ");
                string descuentoStr = Console.ReadLine() ?? string.Empty;
                
                if (double.TryParse(descuentoStr, out double descuento) && descuento >= 0 && descuento <= 100)
                {
                    proveedor.Descuento = descuento;
                    descuentoValido = true;
                }
                else
                {
                    Console.WriteLine("❌ Descuento inválido. Por favor ingrese un número entre 0 y 100.");
                }
            }

            bool tipoDocValido = false;
            while (!tipoDocValido)
            {
                Console.WriteLine("\nTipos de documento disponibles:");
                Console.WriteLine("1. Cédula de Ciudadanía");
                Console.WriteLine("2. NIT");
                Console.WriteLine("3. Pasaporte");
                Console.WriteLine("4. Cédula de Extranjería");
                Console.WriteLine("5. Tarjeta de Identidad");
                Console.Write("\nSeleccione el tipo de documento (1-5): ");
                
                string tipoDocStr = Console.ReadLine() ?? string.Empty;
                if (int.TryParse(tipoDocStr, out int tipoDoc) && tipoDoc >= 1 && tipoDoc <= 5)
                {
                    proveedor.TipoDocumentoId = tipoDoc;
                    tipoDocValido = true;
                }
                else
                {
                    Console.WriteLine("❌ Tipo de documento inválido. Por favor seleccione una opción entre 1 y 5.");
                }
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