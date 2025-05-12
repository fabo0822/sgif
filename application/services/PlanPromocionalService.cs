using sgif.domain.entities;
using sgif.domain.ports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sgif.application.services
{
    public class PlanPromocionalService
    {
        private readonly IPlanPromocionalRepository _planPromocionalRepository;
        private readonly string _connectionString;

        public PlanPromocionalService(IPlanPromocionalRepository planPromocionalRepository, string connectionString)
        {
            _planPromocionalRepository = planPromocionalRepository;
            _connectionString = connectionString;
        }

        public async Task MostrarTodos()
        {
            var planes = await _planPromocionalRepository.GetAll();
            foreach (var plan in planes)
            {
                Console.WriteLine($"Plan #{plan.Id} - {plan.Nombre}");
                Console.WriteLine($"Período: {plan.Inicio:dd/MM/yyyy} - {plan.Fin:dd/MM/yyyy}");
                Console.WriteLine($"Descuento: {plan.Descuento}%");
                Console.WriteLine("------------------------");
            }
        }

        public async Task RegistrarPlan()
        {
            Console.WriteLine("\n=== REGISTRAR NUEVO PLAN PROMOCIONAL ===");
            
            var plan = new PlanPromocional
            {
                Productos = new List<PlanProducto>()
            };

            Console.Write("Nombre del plan: ");
            plan.Nombre = Console.ReadLine();

            Console.Write("Fecha de inicio (dd/MM/yyyy): ");
            plan.Inicio = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", null);

            Console.Write("Fecha de fin (dd/MM/yyyy): ");
            plan.Fin = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", null);

            Console.Write("Porcentaje de descuento: ");
            plan.Descuento = double.Parse(Console.ReadLine());

            bool continuar = true;
            while (continuar)
            {
                var producto = new PlanProducto();
                Console.Write("\nID del Producto: ");
                producto.ProductoId = Console.ReadLine();

                plan.Productos.Add(producto);

                Console.Write("\n¿Agregar otro producto? (S/N): ");
                continuar = Console.ReadLine().ToUpper() == "S";
            }

            await _planPromocionalRepository.Add(plan);
            Console.WriteLine("\n✅ Plan promocional registrado exitosamente.");
        }

        public async Task VerDetalles()
        {
            Console.Write("\nIngrese el ID del plan: ");
            int planId = int.Parse(Console.ReadLine());

            var plan = await _planPromocionalRepository.GetById(planId);
            if (plan == null)
            {
                Console.WriteLine("❌ Plan no encontrado.");
                return;
            }

            Console.WriteLine($"\n=== DETALLES DEL PLAN #{plan.Id} ===");
            Console.WriteLine($"Nombre: {plan.Nombre}");
            Console.WriteLine($"Período: {plan.Inicio:dd/MM/yyyy} - {plan.Fin:dd/MM/yyyy}");
            Console.WriteLine($"Descuento: {plan.Descuento}%");
            Console.WriteLine("\nProductos incluidos:");
            
            var productos = await _planPromocionalRepository.GetProductosPlan(planId);
            foreach (var producto in productos)
            {
                Console.WriteLine($"- Producto ID: {producto.ProductoId}");
            }
        }

        public async Task MostrarPlanesActivos()
        {
            var planes = await _planPromocionalRepository.GetPlanesActivos();
            Console.WriteLine("\n=== PLANES PROMOCIONALES ACTIVOS ===");
            foreach (var plan in planes)
            {
                Console.WriteLine($"Plan #{plan.Id} - {plan.Nombre}");
                Console.WriteLine($"Período: {plan.Inicio:dd/MM/yyyy} - {plan.Fin:dd/MM/yyyy}");
                Console.WriteLine($"Descuento: {plan.Descuento}%");
                Console.WriteLine("------------------------");
            }
        }
    }
} 