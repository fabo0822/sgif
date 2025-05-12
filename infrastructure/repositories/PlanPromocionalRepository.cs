using sgif.domain.entities;
using sgif.domain.ports;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sgif.infrastructure.repositories
{
    public class PlanPromocionalRepository : IPlanPromocionalRepository
    {
        private readonly string _connectionString;

        public PlanPromocionalRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<PlanPromocional>> GetAll()
        {
            var planes = new List<PlanPromocional>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Planes", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                planes.Add(new PlanPromocional
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Inicio = reader.GetDateTime(reader.GetOrdinal("inicio")),
                    Fin = reader.GetDateTime(reader.GetOrdinal("fin")),
                    Descuento = reader.GetDouble(reader.GetOrdinal("descuento"))
                });
            }

            return planes;
        }

        public async Task<PlanPromocional> GetById(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM Planes WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PlanPromocional
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Inicio = reader.GetDateTime(reader.GetOrdinal("inicio")),
                    Fin = reader.GetDateTime(reader.GetOrdinal("fin")),
                    Descuento = reader.GetDouble(reader.GetOrdinal("descuento"))
                };
            }

            return new PlanPromocional
            {
                Id = 0,
                Nombre = string.Empty,
                Inicio = DateTime.MinValue,
                Fin = DateTime.MinValue,
                Descuento = 0.0
            };
        }

        public async Task Add(PlanPromocional plan)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                // Insertar el plan
                using var cmdPlan = new MySqlCommand(
                    "INSERT INTO Planes (nombre, inicio, fin, descuento) VALUES (@nombre, @inicio, @fin, @descuento)",
                    conn, transaction);

                cmdPlan.Parameters.AddWithValue("@nombre", plan.Nombre);
                cmdPlan.Parameters.AddWithValue("@inicio", plan.Inicio);
                cmdPlan.Parameters.AddWithValue("@fin", plan.Fin);
                cmdPlan.Parameters.AddWithValue("@descuento", plan.Descuento);

                await cmdPlan.ExecuteNonQueryAsync();
                var planId = (int)cmdPlan.LastInsertedId;

                // Insertar los productos del plan
                foreach (var producto in plan.Productos)
                {
                    using var cmdProducto = new MySqlCommand(
                        "INSERT INTO PlanProductos (plan_id, producto_id) VALUES (@plan_id, @producto_id)",
                        conn, transaction);

                    cmdProducto.Parameters.AddWithValue("@plan_id", planId);
                    cmdProducto.Parameters.AddWithValue("@producto_id", producto.ProductoId);

                    await cmdProducto.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Update(PlanPromocional plan)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                // Actualizar el plan
                using var cmdPlan = new MySqlCommand(
                    "UPDATE Planes SET nombre = @nombre, inicio = @inicio, fin = @fin, descuento = @descuento WHERE id = @id",
                    conn, transaction);

                cmdPlan.Parameters.AddWithValue("@id", plan.Id);
                cmdPlan.Parameters.AddWithValue("@nombre", plan.Nombre);
                cmdPlan.Parameters.AddWithValue("@inicio", plan.Inicio);
                cmdPlan.Parameters.AddWithValue("@fin", plan.Fin);
                cmdPlan.Parameters.AddWithValue("@descuento", plan.Descuento);

                await cmdPlan.ExecuteNonQueryAsync();

                // Eliminar productos existentes
                using var cmdDelete = new MySqlCommand("DELETE FROM PlanProductos WHERE plan_id = @plan_id", conn, transaction);
                cmdDelete.Parameters.AddWithValue("@plan_id", plan.Id);
                await cmdDelete.ExecuteNonQueryAsync();

                // Insertar los nuevos productos
                foreach (var producto in plan.Productos)
                {
                    using var cmdProducto = new MySqlCommand(
                        "INSERT INTO PlanProductos (plan_id, producto_id) VALUES (@plan_id, @producto_id)",
                        conn, transaction);

                    cmdProducto.Parameters.AddWithValue("@plan_id", plan.Id);
                    cmdProducto.Parameters.AddWithValue("@producto_id", producto.ProductoId);

                    await cmdProducto.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                // Eliminar productos del plan
                using var cmdDeleteProductos = new MySqlCommand("DELETE FROM PlanProductos WHERE plan_id = @plan_id", conn, transaction);
                cmdDeleteProductos.Parameters.AddWithValue("@plan_id", id);
                await cmdDeleteProductos.ExecuteNonQueryAsync();

                // Eliminar el plan
                using var cmdDeletePlan = new MySqlCommand("DELETE FROM Planes WHERE id = @id", conn, transaction);
                cmdDeletePlan.Parameters.AddWithValue("@id", id);
                await cmdDeletePlan.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<PlanPromocional>> GetPlanesActivos()
        {
            var planes = new List<PlanPromocional>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(@"
                SELECT * FROM Planes 
                WHERE inicio <= CURRENT_DATE() AND fin >= CURRENT_DATE()", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                planes.Add(new PlanPromocional
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Inicio = reader.GetDateTime(reader.GetOrdinal("inicio")),
                    Fin = reader.GetDateTime(reader.GetOrdinal("fin")),
                    Descuento = reader.GetDouble(reader.GetOrdinal("descuento"))
                });
            }

            return planes;
        }

        public async Task<List<PlanProducto>> GetProductosPlan(int planId)
        {
            var productos = new List<PlanProducto>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(@"
                SELECT pp.*, p.nombre as producto_nombre 
                FROM PlanProductos pp 
                JOIN Producto p ON pp.producto_id = p.id 
                WHERE pp.plan_id = @plan_id", conn);
            
            cmd.Parameters.AddWithValue("@plan_id", planId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                productos.Add(new PlanProducto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    PlanId = reader.GetInt32(reader.GetOrdinal("plan_id")),
                    ProductoId = reader.GetString(reader.GetOrdinal("producto_id")),
                    Producto = new Producto
                    {
                        Id = reader.GetString(reader.GetOrdinal("producto_id")),
                        Nombre = reader.GetString(reader.GetOrdinal("producto_nombre"))
                    }
                });
            }

            return productos;
        }

        public async Task<List<PlanPromocional>> GetPlanesByFecha(DateTime fecha)
        {
            var planes = new List<PlanPromocional>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(@"
                SELECT * FROM Planes 
                WHERE inicio <= @fecha AND fin >= @fecha", conn);
            
            cmd.Parameters.AddWithValue("@fecha", fecha);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                planes.Add(new PlanPromocional
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Inicio = reader.GetDateTime(reader.GetOrdinal("inicio")),
                    Fin = reader.GetDateTime(reader.GetOrdinal("fin")),
                    Descuento = reader.GetDouble(reader.GetOrdinal("descuento"))
                });
            }

            return planes;
        }
    }
}