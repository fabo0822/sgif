using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgif.domain.ports
{
    public interface IEnvioRepository
    {
         public interface IEnvioRepository : IGenericRepository<Envio> { }
    }
}