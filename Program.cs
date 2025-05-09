using sgif.application.services;
using sgif.domain.entities;
using sgif.domain.factory;
using sgif.infrastructure.mysql;
using sgif.application.UI.Clientes;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string connStr = "server=localhost;database=app;user=root;password=fabo8DEJUNIO@;AllowPublicKeyRetrieval=true;SslMode=none;";
        IDbFactory factory = new MySqlDbFactory(connStr);
        var uiCliente = new UICliente(factory);
        var servicioProducto = new ProductoService(factory.CrearProductoRepository());

        while (true)
        {
            Console.WriteLine("\n--- MENÚ PRINCIPAL ---");
            Console.WriteLine("1. Gestión de Clientes");
            Console.WriteLine("2. Gestión de Productos");
            Console.WriteLine("3. Gestión de Pedidos");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    await uiCliente.MostrarMenu();
                    break;
                case "2":
                    await MenuProductos(servicioProducto);
                    break;
                case "3":
                    await MenuPedidos(factory);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }
    }

    private static async Task MenuClientes(ClienteService servicio)
    {
        while (true)
        {
            Console.WriteLine("\n--- MENÚ CLIENTES ---");
            Console.WriteLine("1. Mostrar todos");
            Console.WriteLine("2. Crear nuevo");
            Console.WriteLine("3. Actualizar");
            Console.WriteLine("4. Eliminar");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    await servicio.MostrarTodos();
                    break;
                case "2":
                    Console.Write("Nombre: ");
                    string nombre = Console.ReadLine() ?? string.Empty;
                    await servicio.CrearCliente(nombre);
                    break;
                case "3":
                    Console.Write("ID del cliente a actualizar: ");
                    int idA = int.Parse(Console.ReadLine()!);
                    Console.Write("Nuevo nombre: ");
                    string nuevoNombre = Console.ReadLine() ?? string.Empty;
                    await servicio.ActualizarCliente(idA, nuevoNombre);
                    break;
                case "4":
                    Console.Write("ID del cliente a eliminar: ");
                    int idE = int.Parse(Console.ReadLine()!);
                    await servicio.EliminarCliente(idE);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }
    }

    private static async Task MenuProductos(ProductoService servicioProducto)
    {
        while (true)
        {
            Console.WriteLine("\n--- MENÚ PRODUCTOS ---");
            Console.WriteLine("1. Mostrar todos");
            Console.WriteLine("2. Crear nuevo");
            Console.WriteLine("3. Actualizar");
            Console.WriteLine("4. Eliminar");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    await servicioProducto.MostrarTodos();
                    break;
                case "2":
                    Producto producto = new Producto();
                    Console.Write("Nombre: ");
                    producto.Nombre = Console.ReadLine();
                    Console.Write("Stock: ");
                    producto.Stock = int.Parse(Console.ReadLine()!);
                    Console.Write("Precio: ");
                    producto.Precio = decimal.Parse(Console.ReadLine()!);
                    await servicioProducto.CrearProducto(producto);
                    break;
                case "3":
                    Console.Write("ID del producto a actualizar: ");
                    int idA = int.Parse(Console.ReadLine()!);
                    Console.Write("Nuevo nombre: ");
                    string nuevoNombre = Console.ReadLine()!;
                    Console.Write("Nuevo stock: ");
                    int nuevoStock = int.Parse(Console.ReadLine()!);
                    Console.Write("Nuevo precio: ");
                    decimal nuevoPrecio = decimal.Parse(Console.ReadLine()!);
                    await servicioProducto.ActualizarProducto(idA, nuevoNombre, nuevoStock, nuevoPrecio);
                    break;
                case "4":
                    Console.Write("ID del producto a eliminar: ");
                    int idE = int.Parse(Console.ReadLine()!);
                    await servicioProducto.EliminarProducto(idE);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }
    }

    private static async Task MenuPedidos(IDbFactory factory)
    {
        var servicioPedido = new PedidoService(factory.CrearPedidoRepository());
        var servicioCliente = new ClienteService(factory.CrearClienteRepository());
        var servicioProducto = new ProductoService(factory.CrearProductoRepository());

        while (true)
        {
            Console.WriteLine("\n--- MENÚ PEDIDOS ---");
            Console.WriteLine("1. Mostrar todos los pedidos");
            Console.WriteLine("2. Crear nuevo pedido");
            Console.WriteLine("3. Ver detalles de un pedido");
            Console.WriteLine("4. Cancelar pedido");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    await servicioPedido.MostrarTodos();
                    break;
                case "2":
                    await CrearNuevoPedido(servicioPedido, servicioCliente, servicioProducto);
                    break;
                case "3":
                    Console.Write("ID del pedido a ver: ");
                    int idPedido = int.Parse(Console.ReadLine()!);
                    await servicioPedido.MostrarDetalles(idPedido);
                    break;
                case "4":
                    Console.Write("ID del pedido a cancelar: ");
                    int idCancelar = int.Parse(Console.ReadLine()!);
                    await servicioPedido.CancelarPedido(idCancelar);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }
    }

    private static async Task CrearNuevoPedido(PedidoService servicioPedido, ClienteService servicioCliente, ProductoService servicioProducto)
    {
        Console.WriteLine("\n--- CREAR NUEVO PEDIDO ---");
        
        // Seleccionar cliente
        Console.WriteLine("\nClientes disponibles:");
        await servicioCliente.MostrarTodos();
        Console.Write("ID del cliente: ");
        int idCliente = int.Parse(Console.ReadLine()!);

        var pedido = new Pedido { ClienteId = idCliente, Estado = "Pendiente" };
        var detalles = new List<DetallePedido>();

        // Agregar productos al pedido
        while (true)
        {
            Console.WriteLine("\nProductos disponibles:");
            await servicioProducto.MostrarTodos();
            Console.Write("ID del producto (0 para terminar): ");
            int idProducto = int.Parse(Console.ReadLine()!);
            
            if (idProducto == 0) break;

            Console.Write("Cantidad: ");
            int cantidad = int.Parse(Console.ReadLine()!);

            var producto = await servicioProducto.ObtenerProducto(idProducto);
            if (producto != null)
            {
                detalles.Add(new DetallePedido
                {
                    ProductoId = idProducto.ToString(),
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = producto.Precio * cantidad
                });
            }
        }

        await servicioPedido.CrearPedido(pedido, detalles);
    }
}