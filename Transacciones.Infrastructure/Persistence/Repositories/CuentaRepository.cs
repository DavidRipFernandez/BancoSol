using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transacciones.Core.Entities;
using Transacciones.Core.Interfaces.Repositories;
using Transacciones.Infrastructure.Persistence.Contexts;

namespace Transacciones.Infrastructure.Persistence.Repositories
{
    public class CuentaRepository : ICuentaRepository
    {
        private readonly TransaccionesDbContext _context;

        public CuentaRepository(TransaccionesDbContext context)
        {
            _context = context;
        }

        public async Task<Cuenta> CreateAsync(Cuenta cuenta)
        {
            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();
            return cuenta;
        }

        public async Task<Cuenta?> GetByIdAsync(int id)
        {
            return await _context.Cuentas.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
