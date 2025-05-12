using sgif.domain.entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sgif.domain.ports
{
    public interface IPlanPromocionalRepository : IGenericRepository<PlanPromocional>
    {
        Task<List<PlanPromocional>> GetPlanesActivos();
        Task<List<PlanProducto>> GetProductosPlan(int planId);
        Task<List<PlanPromocional>> GetPlanesByFecha(DateTime fecha);
    }
} 