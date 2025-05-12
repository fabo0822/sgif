using sgif.domain.entities;
using sgif.domain.ports;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace sgif.infrastructure.repositories
{
    public class CompraRepository : ICompraRepository
    {
        private readonly string _connectionString;

        public CompraRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Compra>> GetAll()
        {
            var compras = new List<Compra>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Compras", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                compras.Add(new Compra
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroProveedorId = reader.GetString(reader.GetOrdinal("tercero_prov_id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TerceroEmpleadoId = reader.GetString(reader.GetOrdinal("tercero_empl_id")),
                    DocCompra = reader.GetString(reader.GetOrdinal("doc_compra"))
                });
            }

            return compras;
        }

        public async Task<Compra> GetById(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Compras WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Compra
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroProveedorId = reader.GetString(reader.GetOrdinal("tercero_prov_id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TerceroEmpleadoId = reader.GetString(reader.GetOrdinal("tercero_empl_id")),
                    DocCompra = reader.GetString(reader.GetOrdinal("doc_compra"))
                };
            }

            return null;
        }

        public async Task Add(Compra compra)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            // Asumimos que la compra tiene al menos un detalle
            var detalle = compra.Detalles.First();
            
            using var cmd = new MySqlCommand("CALL insertar_compra_y_detalle(@proveedor_id, @empleado_id, @fecha_compra, @descripcion, @fecha_detalle, @producto_id, @cantidad, @valor)", conn);
            
            cmd.Parameters.AddWithValue("@proveedor_id", compra.TerceroProveedorId);
            cmd.Parameters.AddWithValue("@empleado_id", compra.TerceroEmpleadoId);
            cmd.Parameters.AddWithValue("@fecha_compra", compra.Fecha);
            cmd.Parameters.AddWithValue("@descripcion", compra.DocCompra);
            cmd.Parameters.AddWithValue("@fecha_detalle", detalle.Fecha);
            cmd.Parameters.AddWithValue("@producto_id", detalle.ProductoId);
            cmd.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
            cmd.Parameters.AddWithValue("@valor", detalle.Valor);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Update(Compra compra)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            // Asumimos que la compra tiene al menos un detalle
            var detalle = compra.Detalles.First();
            
            using var cmd = new MySqlCommand("CALL sp_ActualizarCompra(@id, @tercero_prov_id, @fecha, @tercero_empl_id, @desc_compra, @detalle_fecha, @producto_id, @cantidad, @valor)", conn);
            
            cmd.Parameters.AddWithValue("@id", compra.Id);
            cmd.Parameters.AddWithValue("@tercero_prov_id", compra.TerceroProveedorId);
            cmd.Parameters.AddWithValue("@fecha", compra.Fecha);
            cmd.Parameters.AddWithValue("@tercero_empl_id", compra.TerceroEmpleadoId);
            cmd.Parameters.AddWithValue("@desc_compra", compra.DocCompra);
            cmd.Parameters.AddWithValue("@detalle_fecha", detalle.Fecha);
            cmd.Parameters.AddWithValue("@producto_id", detalle.ProductoId);
            cmd.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
            cmd.Parameters.AddWithValue("@valor", detalle.Valor);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("DELETE FROM Compras WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Compra>> GetComprasByProveedor(string proveedorId)
        {
            var compras = new List<Compra>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Compras WHERE tercero_prov_id = @proveedor_id", conn);
            cmd.Parameters.AddWithValue("@proveedor_id", proveedorId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                compras.Add(new Compra
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroProveedorId = reader.GetString(reader.GetOrdinal("tercero_prov_id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TerceroEmpleadoId = reader.GetString(reader.GetOrdinal("tercero_empl_id")),
                    DocCompra = reader.GetString(reader.GetOrdinal("doc_compra"))
                });
            }

            return compras;
        }

        public async Task<List<Compra>> GetComprasByEmpleado(string empleadoId)
        {
            var compras = new List<Compra>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Compras WHERE tercero_empl_id = @empleado_id", conn);
            cmd.Parameters.AddWithValue("@empleado_id", empleadoId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                compras.Add(new Compra
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    TerceroProveedorId = reader.GetString(reader.GetOrdinal("tercero_prov_id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TerceroEmpleadoId = reader.GetString(reader.GetOrdinal("tercero_empl_id")),
                    DocCompra = reader.GetString(reader.GetOrdinal("doc_compra"))
                });
            }

            return compras;
        }

        public async Task<List<DetalleCompra>> GetDetallesCompra(int compraId)
        {
            var detalles = new List<DetalleCompra>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Detalle_Compra WHERE compra_id = @compra_id", conn);
            cmd.Parameters.AddWithValue("@compra_id", compraId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                detalles.Add(new DetalleCompra
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    ProductoId = reader.GetString(reader.GetOrdinal("producto_id")),
                    Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                    Valor = reader.GetDecimal(reader.GetOrdinal("valor")),
                    CompraId = reader.GetInt32(reader.GetOrdinal("compra_id")),
                    EntradaSalida = reader.GetString(reader.GetOrdinal("entrada_salida"))
                });
            }

            return detalles;
        }
    }
} 