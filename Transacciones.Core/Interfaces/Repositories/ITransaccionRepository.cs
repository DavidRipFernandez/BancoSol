using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transacciones.Core.Entities;

namespace Transacciones.Core.Interfaces.Repositories
{
    public interface ITransaccionRepository
    {
        Task<IEnumerable<Transaccion>> GetByCuentaIdAsync(int cuentaId);
        Task<Transaccion> CreateAsync(Transaccion transaccion);
    }
}
