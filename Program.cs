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

internal class Program
{
    private static async Task Main(string[] args)
    {
        string connStr = "server=localhost;database=dbsgi;user=root;password=fabo8DEJUNIO@;AllowPublicKeyRetrieval=true;SslMode=none;";
        IDbFactory factory = new MySqlDbFactory(connStr);
        var uiCliente = new UICliente(factory, connStr);
        var servicioProducto = new ProductoService(connStr, factory.CrearProductoRepository());
        var servicioEmpleado = new EmpleadoService(factory.CrearEmpleadoRepository(), connStr);
        var proveedorRepository = new ProveedorRepository(connStr);
        var proveedorService = new ProveedorService(proveedorRepository, connStr);

        try
        {
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();
            Console.WriteLine("✅ Conexión a la base de datos exitosa.");
            
            // Inicializar datos necesarios
            await InicializarDatos(conn);
            
            await conn.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error al conectar con la base de datos:");
            Console.WriteLine(ex.Message);
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("\n=== SISTEMA DE GESTIÓN INTEGRAL ===");
            Console.WriteLine("1. Gestión de Empleados");
            Console.WriteLine("2. Gestión de Proveedores");
            Console.WriteLine("3. Gestión de Productos");
            Console.WriteLine("4. Gestión de Clientes");
            Console.WriteLine("0. Salir");
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
                    await MenuProveedores(proveedorService);
                    break;
                case "3":
                Console.Clear();
                    await MenuProductos(servicioProducto);
                    break;
                case "4":
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
}