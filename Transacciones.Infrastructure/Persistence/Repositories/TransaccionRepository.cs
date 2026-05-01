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
    public class TransaccionRepository : ITransaccionRepository
    {
        private readonly TransaccionesDbContext _context;
        public TransaccionRepository(TransaccionesDbContext context)
        {
            _context = context;
        }
        public async Task<Transaccion> CreateAsync(Transaccion transaccion)
        {
            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();
            return transaccion;
        }

        public async Task<IEnumerable<Transaccion>> GetByCuentaIdAsync(int cuentaId)
        {
            return await _context.Transacciones
                .Where(t => t.CuentaId == cuentaId)
                .OrderByDescending(t => t.FechaTransaccion)
                .ToListAsync();
        }
    }
}
