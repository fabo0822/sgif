using sgif.domain.entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sgif.domain.ports
{
    public interface IMovimientoCajaRepository : IGenericRepository<MovimientoCaja>
    {
        Task<List<MovimientoCaja>> GetMovimientosByFecha(DateTime fecha);
        Task<List<MovimientoCaja>> GetMovimientosByTipo(int tipoId);
        Task<decimal> GetSaldoCaja();
    }
} 