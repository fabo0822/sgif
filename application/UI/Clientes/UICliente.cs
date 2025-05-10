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
            _servicio = new ClienteService(factory.CrearClienteRepository(), connectionString);
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
                        await _servicio.MostrarTodos();
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
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine()!;
            await _servicio.CrearCliente(nombre);
        }

        private async Task ActualizarCliente()
        {
            Console.Clear();
            Console.WriteLine("--- ACTUALIZAR CLIENTE ---");
            Console.Write("ID del cliente a actualizar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("Nuevo nombre: ");
                string nuevoNombre = Console.ReadLine()!;
                await _servicio.ActualizarCliente(id, nuevoNombre);
            }
            else
            {
                Console.WriteLine("❌ ID inválido.");
            }
        }

        private async Task EliminarCliente()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR CLIENTE ---");
            Console.Write("ID del cliente a eliminar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                await _servicio.EliminarCliente(id);
            }
            else
            {
                Console.WriteLine("❌ ID inválido.");
            }
        }
    }
}