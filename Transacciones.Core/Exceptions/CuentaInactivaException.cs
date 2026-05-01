using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transacciones.Core.Exceptions
{
    public class CuentaInactivaException : Exception
    {
        public CuentaInactivaException(int cuentaId)
        : base($"La cuenta con ID {cuentaId} está inactiva y no puede realizar transacciones.") { }
    }
}
