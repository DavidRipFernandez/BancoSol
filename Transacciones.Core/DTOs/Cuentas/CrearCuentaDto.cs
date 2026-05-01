using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transacciones.Core.DTOs.Cuentas
{
    public class CrearCuentaDto
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public string Titular { get; set; } = string.Empty;
    }
}
