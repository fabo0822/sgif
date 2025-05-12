using sgif.domain.entities;
using sgif.domain.ports;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;

namespace sgif.infrastructure.repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly string _connectionString;

        public VentaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Venta>> GetAll()
        {
            var ventas = new List<Venta>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Venta", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var venta = new Venta
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                    Fecha = reader.IsDBNull(reader.GetOrdinal("fecha")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TerceroEmpId = reader.IsDBNull(reader.GetOrdinal("tercero_empl_id")) ? string.Empty : reader.GetString(reader.GetOrdinal("tercero_empl_id")),
                    ClienteId = reader.IsDBNull(reader.GetOrdinal("cliente_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("cliente_id")),
                    FactId = reader.IsDBNull(reader.GetOrdinal("fact_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("fact_id")),
                    Detalles = new List<DetalleVenta>()
                };

                ventas.Add(venta);
            }

            reader.Close(); // Cerrar el primer DataReader antes de abrir el segundo

            foreach (var venta in ventas)
            {
                using var cmdDetalles = new MySqlCommand("SELECT * FROM Detalle_Venta WHERE venta_id = @venta_id", conn);
                cmdDetalles.Parameters.AddWithValue("@venta_id", venta.Id);
                using var readerDetalles = await cmdDetalles.ExecuteReaderAsync();

                while (await readerDetalles.ReadAsync())
                {
                    venta.Detalles.Add(new DetalleVenta
                    {
                        Id = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("id")) ? 0 : readerDetalles.GetInt32(readerDetalles.GetOrdinal("id")),
                        VentaId = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("venta_id")) ? 0 : readerDetalles.GetInt32(readerDetalles.GetOrdinal("venta_id")),
                        ProductoId = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("producto_id")) ? string.Empty : readerDetalles.GetString(readerDetalles.GetOrdinal("producto_id")),
                        Cantidad = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("cantidad")) ? 0 : readerDetalles.GetInt32(readerDetalles.GetOrdinal("cantidad")),
                        PrecioUnitario = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("valor")) ? 0 : readerDetalles.GetDecimal(readerDetalles.GetOrdinal("valor"))
                    });
                }
            }

            return ventas;
        }

        public async Task<Venta> GetById(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Venta WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var venta = new Venta
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TerceroEmpId = reader.IsDBNull(reader.GetOrdinal("tercero_empl_id")) ? string.Empty : reader.GetString(reader.GetOrdinal("tercero_empl_id")),
                    ClienteId = reader.IsDBNull(reader.GetOrdinal("cliente_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("cliente_id")),
                    FactId = reader.IsDBNull(reader.GetOrdinal("fact_id")) ? null : reader.GetInt32(reader.GetOrdinal("fact_id")),
                    Detalles = new List<DetalleVenta>()
                };

                reader.Close(); // Cerrar el primer DataReader antes de abrir el segundo

                using var cmdDetalles = new MySqlCommand("SELECT * FROM Detalle_Venta WHERE venta_id = @venta_id", conn);
                cmdDetalles.Parameters.AddWithValue("@venta_id", venta.Id);
                using var readerDetalles = await cmdDetalles.ExecuteReaderAsync();

                while (await readerDetalles.ReadAsync())
                {
                    venta.Detalles.Add(new DetalleVenta
                    {
                        Id = readerDetalles.GetInt32(readerDetalles.GetOrdinal("id")),
                        VentaId = readerDetalles.GetInt32(readerDetalles.GetOrdinal("venta_id")),
                        ProductoId = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("producto_id")) ? string.Empty : readerDetalles.GetString(readerDetalles.GetOrdinal("producto_id")),
                        Cantidad = readerDetalles.GetInt32(readerDetalles.GetOrdinal("cantidad")),
                        PrecioUnitario = readerDetalles.GetDecimal(readerDetalles.GetOrdinal("valor"))
                    });
                }

                return venta;
            }

            return new Venta(); // Reemplazar el retorno nulo con una instancia vacía de Venta
        }

        public async Task Add(Venta venta)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            // Convertir los detalles a JSON
            var detallesJson = JsonSerializer.Serialize(venta.Detalles.Select(d => new
            {
                productoId = d.ProductoId,
                cantidad = d.Cantidad,
                precioUnitario = d.PrecioUnitario
            }));

            using var cmd = new MySqlCommand("CALL RegistrarVenta(@fecha, @cliente, @empleado, @forma_pago, @detalles)", conn);
            
            cmd.Parameters.AddWithValue("@fecha", venta.Fecha);
            cmd.Parameters.AddWithValue("@cliente", venta.ClienteId);
            cmd.Parameters.AddWithValue("@empleado", venta.TerceroEmpId);
            cmd.Parameters.AddWithValue("@forma_pago", "EFECTIVO"); // Valor por defecto
            cmd.Parameters.AddWithValue("@detalles", detallesJson);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Update(Venta venta)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(
                "UPDATE Venta SET fecha = @fecha, tercero_empl_id = @tercero_empl_id, cliente_id = @cliente_id, fact_id = @fact_id WHERE id = @id",
                conn);

            cmd.Parameters.AddWithValue("@id", venta.Id);
            cmd.Parameters.AddWithValue("@fecha", venta.Fecha);
            cmd.Parameters.AddWithValue("@tercero_empl_id", venta.TerceroEmpId);
            cmd.Parameters.AddWithValue("@cliente_id", venta.ClienteId);
            cmd.Parameters.AddWithValue("@fact_id", venta.FactId.HasValue ? (object)venta.FactId.Value : DBNull.Value);

            await cmd.ExecuteNonQueryAsync();

            // Actualizar detalles
            foreach (var detalle in venta.Detalles)
            {
                using var cmdDetalle = new MySqlCommand(
                    "UPDATE Detalle_Venta SET cantidad = @cantidad, valor = @valor " +
                    "WHERE venta_id = @venta_id AND producto_id = @producto_id",
                    conn);

                cmdDetalle.Parameters.AddWithValue("@venta_id", venta.Id);
                cmdDetalle.Parameters.AddWithValue("@producto_id", detalle.ProductoId);
                cmdDetalle.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
                cmdDetalle.Parameters.AddWithValue("@valor", detalle.PrecioUnitario);

                await cmdDetalle.ExecuteNonQueryAsync();
            }
        }

        public async Task Delete(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            // Primero eliminar los detalles
            using var cmdDetalles = new MySqlCommand("DELETE FROM Detalle_Venta WHERE venta_id = @id", conn);
            cmdDetalles.Parameters.AddWithValue("@id", id);
            await cmdDetalles.ExecuteNonQueryAsync();

            // Luego eliminar la venta
            using var cmd = new MySqlCommand("DELETE FROM Venta WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Venta>> GetVentasByCliente(string clienteId)
        {
            var ventas = new List<Venta>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Venta WHERE cliente_id = @cliente_id", conn);
            cmd.Parameters.AddWithValue("@cliente_id", int.Parse(clienteId));

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var venta = new Venta
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TerceroEmpId = reader.IsDBNull(reader.GetOrdinal("tercero_empl_id")) ? string.Empty : reader.GetString(reader.GetOrdinal("tercero_empl_id")),
                    ClienteId = reader.IsDBNull(reader.GetOrdinal("cliente_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("cliente_id")),
                    FactId = reader.IsDBNull(reader.GetOrdinal("fact_id")) ? null : reader.GetInt32(reader.GetOrdinal("fact_id")),
                    Detalles = new List<DetalleVenta>()
                };

                ventas.Add(venta);
            }

            reader.Close(); // Cerrar el primer DataReader antes de abrir el segundo

            foreach (var venta in ventas)
            {
                using var cmdDetalles = new MySqlCommand("SELECT * FROM Detalle_Venta WHERE venta_id = @venta_id", conn);
                cmdDetalles.Parameters.AddWithValue("@venta_id", venta.Id);
                using var readerDetalles = await cmdDetalles.ExecuteReaderAsync();

                while (await readerDetalles.ReadAsync())
                {
                    venta.Detalles.Add(new DetalleVenta
                    {
                        Id = readerDetalles.GetInt32(readerDetalles.GetOrdinal("id")),
                        VentaId = readerDetalles.GetInt32(readerDetalles.GetOrdinal("venta_id")),
                        ProductoId = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("producto_id")) ? string.Empty : readerDetalles.GetString(readerDetalles.GetOrdinal("producto_id")),
                        Cantidad = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("cantidad")) ? 0 : readerDetalles.GetInt32(readerDetalles.GetOrdinal("cantidad")),
                        PrecioUnitario = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("valor")) ? 0 : readerDetalles.GetDecimal(readerDetalles.GetOrdinal("valor"))
                    });
                }
            }

            return ventas;
        }

        public async Task<List<Venta>> GetVentasByEmpleado(string empleadoId)
        {
            var ventas = new List<Venta>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Venta WHERE tercero_empl_id = @empleado_id", conn);
            cmd.Parameters.AddWithValue("@empleado_id", empleadoId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var venta = new Venta
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TerceroEmpId = reader.IsDBNull(reader.GetOrdinal("tercero_empl_id")) ? string.Empty : reader.GetString(reader.GetOrdinal("tercero_empl_id")),
                    ClienteId = reader.IsDBNull(reader.GetOrdinal("cliente_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("cliente_id")),
                    FactId = reader.IsDBNull(reader.GetOrdinal("fact_id")) ? null : reader.GetInt32(reader.GetOrdinal("fact_id")),
                    Detalles = new List<DetalleVenta>()
                };

                ventas.Add(venta);
            }

            reader.Close(); // Cerrar el primer DataReader antes de abrir el segundo

            foreach (var venta in ventas)
            {
                using var cmdDetalles = new MySqlCommand("SELECT * FROM Detalle_Venta WHERE venta_id = @venta_id", conn);
                cmdDetalles.Parameters.AddWithValue("@venta_id", venta.Id);
                using var readerDetalles = await cmdDetalles.ExecuteReaderAsync();

                while (await readerDetalles.ReadAsync())
                {
                    venta.Detalles.Add(new DetalleVenta
                    {
                        Id = readerDetalles.GetInt32(readerDetalles.GetOrdinal("id")),
                        VentaId = readerDetalles.GetInt32(readerDetalles.GetOrdinal("venta_id")),
                        ProductoId = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("producto_id")) ? string.Empty : readerDetalles.GetString(readerDetalles.GetOrdinal("producto_id")),
                        Cantidad = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("cantidad")) ? 0 : readerDetalles.GetInt32(readerDetalles.GetOrdinal("cantidad")),
                        PrecioUnitario = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("valor")) ? 0 : readerDetalles.GetDecimal(readerDetalles.GetOrdinal("valor"))
                    });
                }
            }

            return ventas;
        }

        public async Task<List<DetalleVenta>> GetDetallesVenta(int ventaId)
        {
            var detalles = new List<DetalleVenta>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Detalle_Venta WHERE venta_id = @venta_id", conn);
            cmd.Parameters.AddWithValue("@venta_id", ventaId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                detalles.Add(new DetalleVenta
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    VentaId = reader.GetInt32(reader.GetOrdinal("venta_id")),
                    ProductoId = reader.IsDBNull(reader.GetOrdinal("producto_id")) ? string.Empty : reader.GetString(reader.GetOrdinal("producto_id")),
                    Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                    PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("valor"))
                });
            }

            return detalles;
        }
    }
}