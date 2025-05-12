using sgif.domain.entities;
using sgif.domain.ports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sgif.application.services
{
    public class CompraService
    {
        private readonly ICompraRepository _compraRepository;
        private readonly ITerceroRepository _terceroRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly string _connectionString;

        public CompraService(ICompraRepository compraRepository, ITerceroRepository terceroRepository, IProductoRepository productoRepository, string connectionString)
        {
            _compraRepository = compraRepository;
            _terceroRepository = terceroRepository;
            _productoRepository = productoRepository;
            _connectionString = connectionString;
        }

        public async Task MostrarTodas()
        {
            var compras = await _compraRepository.GetAll();
            foreach (var compra in compras)
            {
                Console.WriteLine($"Compra #{compra.Id} - Fecha: {compra.Fecha:dd/MM/yyyy}");
                Console.WriteLine($"Proveedor ID: {compra.TerceroProveedorId}");
                Console.WriteLine($"Documento: {compra.DocCompra}");
                Console.WriteLine("------------------------");
            }
        }

        public async Task RegistrarCompra()
        {
            Console.WriteLine("\n=== REGISTRAR NUEVA COMPRA ===");

            var compra = new Compra
            {
                Fecha = DateTime.Now,
                Detalles = new List<DetalleCompra>()
            };

            Console.Write("ID del Proveedor: ");
            var proveedorId = Console.ReadLine();
            if (string.IsNullOrEmpty(proveedorId))
            {
                Console.WriteLine("❌ ID del Proveedor no puede estar vacío.");
                return;
            }

            var proveedor = await _terceroRepository.GetById(proveedorId);
            if (proveedor == null)
            {
                Console.WriteLine("❌ Proveedor no encontrado.");
                return;
            }
            compra.TerceroProveedorId = proveedorId;

            Console.Write("ID del Empleado: ");
            var empleadoId = Console.ReadLine();
            if (string.IsNullOrEmpty(empleadoId))
            {
                Console.WriteLine("❌ ID del Empleado no puede estar vacío.");
                return;
            }

            var empleado = await _terceroRepository.GetById(empleadoId);
            if (empleado == null)
            {
                Console.WriteLine("❌ Empleado no encontrado.");
                return;
            }
            compra.TerceroEmpleadoId = empleadoId;

            Console.Write("Número de Documento: ");
            compra.DocCompra = Console.ReadLine() ?? string.Empty;

            bool continuar = true;
            while (continuar)
            {
                var detalle = new DetalleCompra();
                Console.Write("\nID del Producto: ");
                var productoId = Console.ReadLine();
                if (string.IsNullOrEmpty(productoId))
                {
                    Console.WriteLine("❌ ID del Producto no puede estar vacío.");
                    continue;
                }

                var producto = await _productoRepository.GetById(productoId);
                if (producto == null)
                {
                    Console.WriteLine("❌ Producto no encontrado.");
                    continue;
                }
                detalle.ProductoId = productoId;

                Console.Write("Cantidad: ");
                if (!int.TryParse(Console.ReadLine(), out int cantidad))
                {
                    Console.WriteLine("❌ Cantidad inválida.");
                    continue;
                }
                detalle.Cantidad = cantidad;

                Console.Write("Valor unitario: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal valor))
                {
                    Console.WriteLine("❌ Valor unitario inválido.");
                    continue;
                }
                detalle.Valor = valor;

                Console.Write("Tipo (ENTRADA/SALIDA): ");
                var tipo = Console.ReadLine();
                if (string.IsNullOrEmpty(tipo))
                {
                    Console.WriteLine("❌ Tipo no puede estar vacío.");
                    continue;
                }
                detalle.EntradaSalida = tipo.ToUpper();

                compra.Detalles.Add(detalle);

                Console.Write("\n¿Agregar otro producto? (S/N): ");
                var respuesta = Console.ReadLine();
                continuar = !string.IsNullOrEmpty(respuesta) && respuesta.ToUpper() == "S";
            }

            await _compraRepository.Add(compra);
            Console.WriteLine("\n✅ Compra registrada exitosamente.");
        }

        public async Task VerDetalles()
        {
            Console.Write("\nIngrese el ID de la compra: ");
            if (!int.TryParse(Console.ReadLine(), out int compraId))
            {
                Console.WriteLine("❌ ID de la compra inválido.");
                return;
            }

            var compra = await _compraRepository.GetById(compraId);
            if (compra == null)
            {
                Console.WriteLine("❌ Compra no encontrada.");
                return;
            }

            Console.WriteLine($"\n=== DETALLES DE COMPRA #{compra.Id} ===");
            Console.WriteLine($"Fecha: {compra.Fecha:dd/MM/yyyy}");
            Console.WriteLine($"Proveedor ID: {compra.TerceroProveedorId}");
            Console.WriteLine($"Empleado ID: {compra.TerceroEmpleadoId}");
            Console.WriteLine($"Documento: {compra.DocCompra}");
            Console.WriteLine("\nProducto:");

            var detalles = await _compraRepository.GetDetallesCompra(compraId);
            foreach (var detalle in detalles)
            {
                Console.WriteLine($"- Producto: {detalle.ProductoId}");
                Console.WriteLine($"  Cantidad: {detalle.Cantidad}");
                Console.WriteLine($"  Valor: ${detalle.Valor}");
                Console.WriteLine($"  Tipo: {detalle.EntradaSalida}");
            }
        }
    }
}