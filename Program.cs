using sgif.application.services;
using sgif.domain.entities;
using sgif.domain.factory;
using sgif.infrastructure.mysql;
using sgif.application.UI.Clientes;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using sgif.infrastructure.repositories;
using System.IO;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static IConfiguration _configuration;

    private static async Task Main(string[] args)
    {
        try
        {
            // Configuración
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connStr = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connStr))
            {
                throw new Exception("No se encontró la cadena de conexión en la configuración.");
            }

            // Inicialización de servicios
            IDbFactory factory = new MySqlDbFactory(connStr);
            var uiCliente = new UICliente(factory, connStr);
            var servicioProducto = new ProductoService(connStr, factory.CrearProductoRepository());
            var servicioEmpleado = new EmpleadoService(factory.CrearEmpleadoRepository(), connStr);
            var proveedorRepository = new ProveedorRepository(connStr);
            var terceroRepository = new TerceroRepository(connStr);
            var servicioProveedor = new ProveedorService(proveedorRepository, terceroRepository, connStr);
            var ventaRepository = new VentaRepository(connStr);
            var clienteRepository = new ClienteRepository(connStr);
            var servicioCliente = new ClienteService(clienteRepository, terceroRepository, connStr);
            var servicioVenta = new VentaService(ventaRepository, factory.CrearProductoRepository(), terceroRepository, clienteRepository);
            var compraRepository = new CompraRepository(connStr);
            var productoRepository = new ProductoRepository(connStr);
            var servicioCompra = new CompraService(compraRepository, terceroRepository, productoRepository, connStr);
            var movimientoCajaRepository = new MovimientoCajaRepository(connStr);
            var servicioMovimientoCaja = new MovimientoCajaService(movimientoCajaRepository, connStr);
            var planPromocionalRepository = new PlanPromocionalRepository(connStr);
            var servicioPlanPromocional = new PlanPromocionalService(planPromocionalRepository, connStr);

            // Verificar conexión a la base de datos
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();
            Console.WriteLine("✅ Conexión a la base de datos exitosa.");
            
            // Inicializar datos necesarios
            await InicializarDatos(conn);
            
            await conn.CloseAsync();

            // Menú principal
            await MostrarMenuPrincipal(servicioProducto, servicioVenta, servicioCompra, 
                servicioMovimientoCaja, servicioPlanPromocional, servicioEmpleado, 
                servicioProveedor, uiCliente);
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error crítico en la aplicación:");
            Console.WriteLine(ex.Message);
            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }
    }

    private static async Task MostrarMenuPrincipal(
        ProductoService servicioProducto,
        VentaService servicioVenta,
        CompraService servicioCompra,
        MovimientoCajaService servicioMovimientoCaja,
        PlanPromocionalService servicioPlanPromocional,
        EmpleadoService servicioEmpleado,
        ProveedorService servicioProveedor,
        UICliente uiCliente)
    {
        while (true)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("\n=== SISTEMA DE GESTIÓN INTEGRAL ===");
                Console.WriteLine("1. Gestión de Productos");
                Console.WriteLine("2. Gestión de Ventas");
                Console.WriteLine("3. Gestión de Compras");
                Console.WriteLine("4. Movimientos de Caja");
                Console.WriteLine("5. Gestión de Planes Promocionales");
                Console.WriteLine("6. Gestión de Personas");
                Console.WriteLine("0. Salir");
                Console.Write("Opción: ");
                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        Console.Clear();
                        await MenuProductos(servicioProducto);
                        break;
                    case "2":
                        Console.Clear();
                        await MenuVentas(servicioVenta);
                        break;
                    case "3":
                        Console.Clear();
                        await MenuCompras(servicioCompra);
                        break;
                    case "4":
                        Console.Clear();
                        await MenuMovimientosCaja(servicioMovimientoCaja);
                        break;
                    case "5":
                        Console.Clear();
                        await MenuPlanesPromocionales(servicioPlanPromocional);
                        break;
                    case "6":
                        Console.Clear();
                        await MenuPersonas(servicioEmpleado, servicioProveedor, uiCliente);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("❌ Opción inválida.");
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
    }

    private static async Task MenuEmpleados(EmpleadoService servicio)
    {
        while (true)
        {
            Console.WriteLine("\n--- MENÚ EMPLEADOS ---");
            Console.WriteLine("1. Mostrar todos los empleados");
            Console.WriteLine("2. Registrar nuevo empleado");
            Console.WriteLine("3. Actualizar empleado");
            Console.WriteLine("4. Eliminar empleado");
            Console.WriteLine("5. Ver detalles de empleado");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                 Console.Clear();
                    await servicio.MostrarTodos();
                    break;
                case "2":
                 Console.Clear();
                    await servicio.RegistrarEmpleado();
                    break;
                case "3":
                 Console.Clear();
                    await servicio.ActualizarEmpleado();
                    break;
                case "4":
                 Console.Clear();
                    await servicio.EliminarEmpleado();
                    break;
                case "5":
                 Console.Clear();
                    await servicio.VerDetalles();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }
    }

    private static async Task MenuProveedores(ProveedorService servicio)
    {
        while (true)
        {
            Console.WriteLine("\n=== MENÚ PRINCIPAL ===");
            Console.WriteLine("1. Registrar Proveedor");
            Console.WriteLine("2. Listar Proveedores");
            Console.WriteLine("3. Actualizar Proveedor");
            Console.WriteLine("4. Eliminar Proveedor");
            Console.WriteLine("5. Ver Productos del Proveedor");
            Console.WriteLine("0. Salir");
            Console.Write("\nSeleccione una opción: ");

            var opcion = Console.ReadLine();

            try
            {
                switch (opcion)
                {
                    case "1":
                    Console.Clear();
                        await servicio.RegistrarProveedor();
                        break;
                    case "2":
                    Console.Clear();
                        await servicio.ListarProveedores();
                        break;
                    case "3":
                    Console.Clear();
                        await servicio.ActualizarProveedor();
                        break;
                    case "4":
                    Console.Clear();
                        await servicio.EliminarProveedor();
                        break;
                    case "5":
                    Console.Clear();
                        await servicio.VerProductosProveedor();
                        break;
                    case "0":
                        Console.WriteLine("\n¡Hasta pronto!");
                        return;
                    default:
                        Console.WriteLine("\nOpción no válida. Presione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
    }

    private static async Task MenuProductos(ProductoService servicioProducto)
    {
        while (true)
        {
            Console.WriteLine("\n--- MENÚ PRODUCTOS ---");
            Console.WriteLine("1. Mostrar todos los productos");
            Console.WriteLine("2. Registrar nuevo producto");
            Console.WriteLine("3. Actualizar producto");
            Console.WriteLine("4. Eliminar producto");
            Console.WriteLine("5. Ver stock y detalles");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                Console.Clear();
                    await servicioProducto.MostrarTodos();
                    break;
                case "2":
                Console.Clear();
                    await servicioProducto.RegistrarProducto();
                    break;
                case "3":
                Console.Clear();
                    await servicioProducto.ActualizarProducto();
                    break;
                case "4":
                Console.Clear();
                    await servicioProducto.EliminarProducto();
                    break;
                case "5":
                Console.Clear();
                    await servicioProducto.VerDetalles();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }
    }

    private static async Task MenuVentas(VentaService servicio)
    {
        while (true)
        {
            Console.WriteLine("\n--- MENÚ VENTAS ---");
            Console.WriteLine("1. Mostrar todas las ventas");
            Console.WriteLine("2. Registrar nueva venta");
            Console.WriteLine("3. Ver detalles de venta");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.Clear();
                    await servicio.MostrarTodas();
                    break;
                case "2":
                    Console.Clear();
                    await servicio.RegistrarVenta();
                    break;
                case "3":
                    Console.Clear();
                    await servicio.VerDetalles();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }
    }

    private static async Task MenuCompras(CompraService servicio)
    {
        while (true)
        {
            Console.WriteLine("\n--- MENÚ COMPRAS ---");
            Console.WriteLine("1. Mostrar todas las compras");
            Console.WriteLine("2. Registrar nueva compra");
            Console.WriteLine("3. Ver detalles de compra");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.Clear();
                    await servicio.MostrarTodas();
                    break;
                case "2":
                    Console.Clear();
                    await servicio.RegistrarCompra();
                    break;
                case "3":
                    Console.Clear();
                    await servicio.VerDetalles();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }
    }

    private static async Task InicializarDatos(MySqlConnection conn)
    {
        try
        {
            // Verificar si ya existen datos
            using var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM TipoDocumento", conn);
            var count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
            
            if (count == 0)
            {
                Console.WriteLine("Inicializando datos en la base de datos...");
                
                // Leer el script SQL
                string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database", "insert_data.sql");
                string script = await File.ReadAllTextAsync(scriptPath);
                
                // Ejecutar el script
                using var cmd = new MySqlCommand(script, conn);
                await cmd.ExecuteNonQueryAsync();
                
                Console.WriteLine("Datos inicializados correctamente.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al inicializar datos: {ex.Message}");
        }
    }

    private static async Task MenuPersonas(EmpleadoService servicioEmpleado, ProveedorService servicioProveedor, UICliente uiCliente)
    {
        while (true)
        {
            Console.WriteLine("\n--- GESTIÓN DE PERSONAS ---");
            Console.WriteLine("1. Gestión de Empleados");
            Console.WriteLine("2. Gestión de Proveedores");
            Console.WriteLine("3. Gestión de Clientes");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.Clear();
                    await MenuEmpleados(servicioEmpleado);
                    break;
                case "2":
                    Console.Clear();
                    await MenuProveedores(servicioProveedor);
                    break;
                case "3":
                    Console.Clear();
                    await uiCliente.MostrarMenu();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }
    }

    private static async Task MenuMovimientosCaja(MovimientoCajaService servicio)
    {
        while (true)
        {
            Console.WriteLine("\n--- MENÚ MOVIMIENTOS DE CAJA ---");
            Console.WriteLine("1. Mostrar todos los movimientos");
            Console.WriteLine("2. Registrar nuevo movimiento");
            Console.WriteLine("3. Ver detalles de movimiento");
            Console.WriteLine("4. Ver saldo actual");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.Clear();
                    await servicio.MostrarTodos();
                    break;
                case "2":
                    Console.Clear();
                    await servicio.RegistrarMovimiento();
                    break;
                case "3":
                    Console.Clear();
                    await servicio.VerDetalles();
                    break;
                case "4":
                    Console.Clear();
                    await servicio.VerSaldoCaja();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }
    }

    private static async Task MenuPlanesPromocionales(PlanPromocionalService servicio)
    {
        while (true)
        {
            Console.WriteLine("\n--- MENÚ PLANES PROMOCIONALES ---");
            Console.WriteLine("1. Mostrar todos los planes");
            Console.WriteLine("2. Registrar nuevo plan");
            Console.WriteLine("3. Ver detalles de plan");
            Console.WriteLine("4. Ver planes activos");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.Clear();
                    await servicio.MostrarTodos();
                    break;
                case "2":
                    Console.Clear();
                    await servicio.RegistrarPlan();
                    break;
                case "3":
                    Console.Clear();
                    await servicio.VerDetalles();
                    break;
                case "4":
                    Console.Clear();
                    await servicio.MostrarPlanesActivos();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }
    }
}