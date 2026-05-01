using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transacciones.Core.Exceptions
{
    public class CuentaNoEncontradaException : Exception
    {
        public CuentaNoEncontradaException(int cuentaId)
       : base($"No se encontró la cuenta con ID {cuentaId}.") { }
    }
}
