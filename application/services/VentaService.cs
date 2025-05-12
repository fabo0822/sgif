using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgif.domain.entities;
using sgif.domain.ports;

namespace sgif.application.services
{
    public class VentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly ITerceroRepository _terceroRepository;
        private readonly IClienteRepository _clienteRepository;

        public VentaService(IVentaRepository ventaRepository, IProductoRepository productoRepository, ITerceroRepository terceroRepository, IClienteRepository clienteRepository)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
            _terceroRepository = terceroRepository;
            _clienteRepository = clienteRepository;
        }

        public async Task MostrarTodas()
        {
            try
            {
                var ventas = await _ventaRepository.GetAll();
                if (ventas == null || !ventas.Any())
                {
                    Console.WriteLine("\nNo hay ventas registradas.");
                    return;
                }

                Console.WriteLine("\n=== LISTA DE VENTAS ===");
                foreach (var venta in ventas)
                {
                    if (venta == null) continue;

                    var cliente = venta.ClienteId > 0 ? await _clienteRepository.GetByIdAsync(venta.ClienteId) : null;
                    var empleado = !string.IsNullOrEmpty(venta.TerceroEmpId) ? await _terceroRepository.GetById(venta.TerceroEmpId) : null;

                    Console.WriteLine($"\nID: {venta.Id}");
                    Console.WriteLine($"Fecha: {venta.Fecha:dd/MM/yyyy}");
                    Console.WriteLine($"Cliente: {(cliente?.Nombre ?? "N/A")}");
                    Console.WriteLine($"Empleado: {(empleado?.Nombre ?? "N/A")}");

                    if (venta.Detalles == null || !venta.Detalles.Any())
                    {
                        Console.WriteLine("Detalles: No hay detalles disponibles");
                        Console.WriteLine("Total: $0.00");
                    }
                    else
                    {
                        decimal total = 0;
                        Console.WriteLine("\nDetalles de productos:");
                        foreach (var detalle in venta.Detalles.Where(d => d != null))
                        {
                            if (detalle.Cantidad <= 0 || detalle.PrecioUnitario <= 0) continue;
                            
                            var producto = !string.IsNullOrEmpty(detalle.ProductoId) ? 
                                await _productoRepository.GetById(detalle.ProductoId) : null;
                            
                            Console.WriteLine($"- {producto?.Nombre ?? "Producto no encontrado"}: " +
                                $"{detalle.Cantidad} x ${detalle.PrecioUnitario:N2} = ${(detalle.Cantidad * detalle.PrecioUnitario):N2}");
                            
                            total += detalle.Cantidad * detalle.PrecioUnitario;
                        }
                        Console.WriteLine($"\nTotal: ${total:N2}");
                    }

                    Console.WriteLine("------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al mostrar ventas: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Detalle: {ex.InnerException.Message}");
                }
            }
            finally
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }

        public async Task RegistrarVenta()
        {
            try
            {
                Console.WriteLine("\n=== REGISTRO DE NUEVA VENTA ===");
                
                // Obtener cliente
                Console.Write("ID del Cliente (entero): ");
                if (!int.TryParse(Console.ReadLine(), out int clienteId))
                {
                    Console.WriteLine("❌ ID de cliente inválido.");
                    return;
                }
                var cliente = await _clienteRepository.GetByIdAsync(clienteId);
                if (cliente == null)
                {
                    Console.WriteLine("❌ Cliente no encontrado. Use el ID entero de la tabla Cliente.");
                    return;
                }

                // Obtener empleado
                Console.Write("ID del Empleado: ");
                if (!int.TryParse(Console.ReadLine(), out int empleadoId))
                {
                    Console.WriteLine("❌ ID de empleado inválido.");
                    return;
                }
                var empleado = await _terceroRepository.GetById(empleadoId.ToString());
                if (empleado == null)
                {
                    Console.WriteLine("❌ Empleado no encontrado.");
                    return;
                }

                var venta = new Venta
                {
                    Fecha = DateTime.Now,
                    ClienteId = clienteId,
                    TerceroEmpId = empleadoId.ToString(),
                    Detalles = new List<DetalleVenta>()
                };

                bool agregarProducto = true;
                while (agregarProducto)
                {
                    Console.Write("\nID del Producto (0 para terminar): ");
                    if (!int.TryParse(Console.ReadLine(), out int productoId) || productoId == 0)
                    {
                        if (venta.Detalles.Any())
                        {
                            agregarProducto = false;
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("❌ La venta debe tener al menos un producto.");
                            continue;
                        }
                    }

                    var producto = await _productoRepository.GetById(productoId);
                    if (producto == null)
                    {
                        Console.WriteLine("❌ Producto no encontrado.");
                        continue;
                    }

                    Console.Write("Cantidad: ");
                    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
                    {
                        Console.WriteLine("❌ Cantidad inválida.");
                        continue;
                    }

                    if (cantidad > producto.Stock)
                    {
                        Console.WriteLine($"❌ Stock insuficiente. Stock disponible: {producto.Stock}");
                        continue;
                    }

                    venta.Detalles.Add(new DetalleVenta
                    {
                        ProductoId = producto.Id,
                        Cantidad = cantidad,
                        PrecioUnitario = producto.PrecioVenta
                    });

                    producto.Stock -= cantidad;
                    await _productoRepository.Update(producto);
                }

                await _ventaRepository.Add(venta);
                Console.WriteLine("\n✅ Venta registrada exitosamente.");
                Console.WriteLine($"Total: ${venta.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario):N2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al registrar venta: {ex.Message}");
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
                Console.WriteLine("\n=== VER DETALLES DE VENTA ===");
                Console.Write("ID de la venta: ");
                if (!int.TryParse(Console.ReadLine(), out int ventaId))
                {
                    Console.WriteLine("❌ ID de venta inválido.");
                    return;
                }

                var venta = await _ventaRepository.GetById(ventaId);
                if (venta == null)
                {
                    Console.WriteLine("❌ Venta no encontrada.");
                    return;
                }

                var cliente = await _terceroRepository.GetById(venta.ClienteId.ToString());
                var empleado = await _terceroRepository.GetById(venta.TerceroEmpId);

                Console.WriteLine("\n=== DETALLES DE LA VENTA ===");
                Console.WriteLine($"ID: {venta.Id}");
                Console.WriteLine($"Fecha: {venta.Fecha:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Cliente: {cliente?.Nombre ?? "N/A"}");
                Console.WriteLine($"Empleado: {empleado?.Nombre ?? "N/A"}");
                Console.WriteLine("\nProductos:");
                foreach (var detalle in venta.Detalles)
                {
                    var producto = await _productoRepository.GetById(detalle.ProductoId);
                    Console.WriteLine($"- {producto?.Nombre ?? "N/A"}: {detalle.Cantidad} x ${detalle.PrecioUnitario:N2} = ${(detalle.Cantidad * detalle.PrecioUnitario):N2}");
                }
                Console.WriteLine($"\nTotal: ${venta.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario):N2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al ver detalles de la venta: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
    }
}