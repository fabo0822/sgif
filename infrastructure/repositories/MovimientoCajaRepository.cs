using sgif.domain.entities;
using sgif.domain.ports;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sgif.infrastructure.repositories
{
    public class MovimientoCajaRepository : IMovimientoCajaRepository
    {
        private readonly string _connectionString;

        public MovimientoCajaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<MovimientoCaja>> GetAll()
        {
            var movimientos = new List<MovimientoCaja>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(@"
                SELECT m.*, t.nombre as tipo_nombre, t.tipo as tipo_tipo 
                FROM MovCaja m 
                JOIN TipoMovCaja t ON m.tipo_mov_id = t.id", conn);
            
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                movimientos.Add(new MovimientoCaja
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TipoMovimientoId = reader.GetInt32(reader.GetOrdinal("tipo_mov_id")),
                    Valor = reader.GetDecimal(reader.GetOrdinal("valor")),
                    Concepto = reader.GetString(reader.GetOrdinal("concepto")),
                    TerceroId = reader.IsDBNull(reader.GetOrdinal("tercero_id")) ? null : reader.GetString(reader.GetOrdinal("tercero_id")),
                    TipoMovimiento = new TipoMovimientoCaja
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("tipo_mov_id")),
                        Nombre = reader.GetString(reader.GetOrdinal("tipo_nombre")),
                        Tipo = reader.GetString(reader.GetOrdinal("tipo_tipo"))
                    }
                });
            }

            return movimientos;
        }

        public async Task<MovimientoCaja> GetById(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(@"
                SELECT m.*, t.nombre as tipo_nombre, t.tipo as tipo_tipo 
                FROM MovCaja m 
                JOIN TipoMovCaja t ON m.tipo_mov_id = t.id 
                WHERE m.id = @id", conn);
            
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new MovimientoCaja
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TipoMovimientoId = reader.GetInt32(reader.GetOrdinal("tipo_mov_id")),
                    Valor = reader.GetDecimal(reader.GetOrdinal("valor")),
                    Concepto = reader.GetString(reader.GetOrdinal("concepto")),
                    TerceroId = reader.IsDBNull(reader.GetOrdinal("tercero_id")) ? null : reader.GetString(reader.GetOrdinal("tercero_id")),
                    TipoMovimiento = new TipoMovimientoCaja
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("tipo_mov_id")),
                        Nombre = reader.GetString(reader.GetOrdinal("tipo_nombre")),
                        Tipo = reader.GetString(reader.GetOrdinal("tipo_tipo"))
                    }
                };
            }

            return new MovimientoCaja();
        }

        public async Task Add(MovimientoCaja movimiento)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(@"
                INSERT INTO MovCaja (fecha, tipo_mov_id, valor, concepto, tercero_id) 
                VALUES (@fecha, @tipo_mov_id, @valor, @concepto, @tercero_id)", conn);

            cmd.Parameters.AddWithValue("@fecha", movimiento.Fecha);
            cmd.Parameters.AddWithValue("@tipo_mov_id", movimiento.TipoMovimientoId);
            cmd.Parameters.AddWithValue("@valor", movimiento.Valor);
            cmd.Parameters.AddWithValue("@concepto", movimiento.Concepto);
            cmd.Parameters.AddWithValue("@tercero_id", movimiento.TerceroId ?? (object)DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Update(MovimientoCaja movimiento)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(@"
                UPDATE MovCaja 
                SET fecha = @fecha, 
                    tipo_mov_id = @tipo_mov_id, 
                    valor = @valor, 
                    concepto = @concepto, 
                    tercero_id = @tercero_id 
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@id", movimiento.Id);
            cmd.Parameters.AddWithValue("@fecha", movimiento.Fecha);
            cmd.Parameters.AddWithValue("@tipo_mov_id", movimiento.TipoMovimientoId);
            cmd.Parameters.AddWithValue("@valor", movimiento.Valor);
            cmd.Parameters.AddWithValue("@concepto", movimiento.Concepto);
            cmd.Parameters.AddWithValue("@tercero_id", movimiento.TerceroId ?? (object)DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("DELETE FROM MovCaja WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<MovimientoCaja>> GetMovimientosByFecha(DateTime fecha)
        {
            var movimientos = new List<MovimientoCaja>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(@"
                SELECT m.*, t.nombre as tipo_nombre, t.tipo as tipo_tipo 
                FROM MovCaja m 
                JOIN TipoMovCaja t ON m.tipo_mov_id = t.id 
                WHERE DATE(m.fecha) = DATE(@fecha)", conn);
            
            cmd.Parameters.AddWithValue("@fecha", fecha);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                movimientos.Add(new MovimientoCaja
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TipoMovimientoId = reader.GetInt32(reader.GetOrdinal("tipo_mov_id")),
                    Valor = reader.GetDecimal(reader.GetOrdinal("valor")),
                    Concepto = reader.GetString(reader.GetOrdinal("concepto")),
                    TerceroId = reader.IsDBNull(reader.GetOrdinal("tercero_id")) ? null : reader.GetString(reader.GetOrdinal("tercero_id")),
                    TipoMovimiento = new TipoMovimientoCaja
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("tipo_mov_id")),
                        Nombre = reader.GetString(reader.GetOrdinal("tipo_nombre")),
                        Tipo = reader.GetString(reader.GetOrdinal("tipo_tipo"))
                    }
                });
            }

            return movimientos;
        }

        public async Task<List<MovimientoCaja>> GetMovimientosByTipo(int tipoId)
        {
            var movimientos = new List<MovimientoCaja>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(@"
                SELECT m.*, t.nombre as tipo_nombre, t.tipo as tipo_tipo 
                FROM MovCaja m 
                JOIN TipoMovCaja t ON m.tipo_mov_id = t.id 
                WHERE m.tipo_mov_id = @tipo_id", conn);
            
            cmd.Parameters.AddWithValue("@tipo_id", tipoId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                movimientos.Add(new MovimientoCaja
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    TipoMovimientoId = reader.GetInt32(reader.GetOrdinal("tipo_mov_id")),
                    Valor = reader.GetDecimal(reader.GetOrdinal("valor")),
                    Concepto = reader.GetString(reader.GetOrdinal("concepto")),
                    TerceroId = reader.IsDBNull(reader.GetOrdinal("tercero_id")) ? null : reader.GetString(reader.GetOrdinal("tercero_id")),
                    TipoMovimiento = new TipoMovimientoCaja
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("tipo_mov_id")),
                        Nombre = reader.GetString(reader.GetOrdinal("tipo_nombre")),
                        Tipo = reader.GetString(reader.GetOrdinal("tipo_tipo"))
                    }
                });
            }

            return movimientos;
        }

        public async Task<decimal> GetSaldoCaja()
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(@"
                SELECT SUM(CASE 
                    WHEN t.tipo = 'INGRESO' THEN m.valor 
                    WHEN t.tipo = 'EGRESO' THEN -m.valor 
                    ELSE 0 
                END) as saldo
                FROM MovCaja m 
                JOIN TipoMovCaja t ON m.tipo_mov_id = t.id", conn);

            var result = await cmd.ExecuteScalarAsync();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }
    }
}