using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transacciones.Core.Entities;

namespace Transacciones.Core.Interfaces.Repositories
{
    public interface ICuentaRepository
    {
        Task<Cuenta?> GetByIdAsync(int id);
        Task<Cuenta> CreateAsync(Cuenta cuenta);
    }
}
