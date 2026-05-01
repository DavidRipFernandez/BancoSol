using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transacciones.Core.Exceptions
{
    public class SaldoInsuficienteException : Exception
    {
        public decimal SaldoDisponible { get; }
        public decimal MontoSolicitado { get; }

        public SaldoInsuficienteException(decimal saldoDisponible, decimal montoSolicitado) : base($"Saldo insuficiente. Disponible: {saldoDisponible:C}, Solicitado: {montoSolicitado:C}")
        {
            SaldoDisponible = saldoDisponible;
            MontoSolicitado = montoSolicitado;
        }
    }
}
