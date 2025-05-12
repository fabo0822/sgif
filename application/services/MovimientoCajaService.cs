using sgif.domain.entities;
using sgif.domain.ports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sgif.application.services
{
    public class MovimientoCajaService
    {
        private readonly IMovimientoCajaRepository _movimientoCajaRepository;
        private readonly string _connectionString;

        public MovimientoCajaService(IMovimientoCajaRepository movimientoCajaRepository, string connectionString)
        {
            _movimientoCajaRepository = movimientoCajaRepository;
            _connectionString = connectionString;
        }

        public async Task MostrarTodos()
        {
            var movimientos = await _movimientoCajaRepository.GetAll();
            foreach (var movimiento in movimientos)
            {
                Console.WriteLine($"Movimiento #{movimiento.Id} - Fecha: {movimiento.Fecha:dd/MM/yyyy}");
                Console.WriteLine($"Tipo: {movimiento.TipoMovimiento.Nombre}");
                Console.WriteLine($"Valor: ${movimiento.Valor}");
                Console.WriteLine($"Concepto: {movimiento.Concepto}");
                Console.WriteLine("------------------------");
            }
        }

        public async Task RegistrarMovimiento()
        {
            Console.WriteLine("\n=== REGISTRAR NUEVO MOVIMIENTO ===");

            var movimiento = new MovimientoCaja
            {
                Fecha = DateTime.Now
            };

            Console.Write("ID del Tipo de Movimiento: ");
            if (!int.TryParse(Console.ReadLine(), out int tipoMovimientoId))
            {
                Console.WriteLine("❌ ID del Tipo de Movimiento inválido.");
                return;
            }
            movimiento.TipoMovimientoId = tipoMovimientoId;

            Console.Write("Valor: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal valor))
            {
                Console.WriteLine("❌ Valor inválido.");
                return;
            }
            movimiento.Valor = valor;

            Console.Write("Concepto: ");
            var concepto = Console.ReadLine();
            if (string.IsNullOrEmpty(concepto))
            {
                Console.WriteLine("❌ Concepto no puede estar vacío.");
                return;
            }
            movimiento.Concepto = concepto;

            Console.Write("ID del Tercero (opcional): ");
            var terceroId = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(terceroId))
            {
                movimiento.TerceroId = terceroId;
            }

            await _movimientoCajaRepository.Add(movimiento);
            Console.WriteLine("\n✅ Movimiento registrado exitosamente.");
        }

        public async Task VerDetalles()
        {
            Console.Write("\nIngrese el ID del movimiento: ");
            if (!int.TryParse(Console.ReadLine(), out int movimientoId))
            {
                Console.WriteLine("❌ ID del movimiento inválido.");
                return;
            }

            var movimiento = await _movimientoCajaRepository.GetById(movimientoId);
            if (movimiento == null)
            {
                Console.WriteLine("❌ Movimiento no encontrado.");
                return;
            }

            Console.WriteLine($"\n=== DETALLES DE MOVIMIENTO #{movimiento.Id} ===");
            Console.WriteLine($"Fecha: {movimiento.Fecha:dd/MM/yyyy}");
            Console.WriteLine($"Tipo: {movimiento.TipoMovimiento.Nombre}");
            Console.WriteLine($"Valor: ${movimiento.Valor}");
            Console.WriteLine($"Concepto: {movimiento.Concepto}");
            if (!string.IsNullOrEmpty(movimiento.TerceroId))
            {
                Console.WriteLine($"Tercero ID: {movimiento.TerceroId}");
            }
        }

        public async Task VerSaldoCaja()
        {
            var saldo = await _movimientoCajaRepository.GetSaldoCaja();
            Console.WriteLine($"\n=== SALDO ACTUAL DE CAJA ===");
            Console.WriteLine($"Saldo: ${saldo}");
        }
    }
}