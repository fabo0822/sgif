using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgif.domain.factory;
using sgif.application.services;

namespace sgif.application.UI.Clientes
{
    public class UICliente
    {
        private readonly ClienteService _servicio;

        public UICliente(IDbFactory factory, string connectionString)
        {
            var terceroRepository = factory.CrearTerceroRepository();
            _servicio = new ClienteService(factory.CrearClienteRepository(), terceroRepository, connectionString);
        }

        public async Task MostrarMenu()
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
                        await _servicio.ListarClientes();
                        break;
                    case "2":
                        await CrearCliente();
                        break;
                    case "3":
                        await ActualizarCliente();
                        break;
                    case "4":
                        await EliminarCliente();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("❌ Opción inválida.");
                        break;
                }
            }
        }

        private async Task CrearCliente()
        {
            Console.Clear();
            Console.WriteLine("--- CREAR NUEVO CLIENTE ---");
            await _servicio.RegistrarCliente();
        }

        private async Task ActualizarCliente()
        {
            Console.Clear();
            Console.WriteLine("--- ACTUALIZAR CLIENTE ---");
            await _servicio.ActualizarCliente();
        }

        private async Task EliminarCliente()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR CLIENTE ---");
            await _servicio.EliminarCliente();
        }
    }
}