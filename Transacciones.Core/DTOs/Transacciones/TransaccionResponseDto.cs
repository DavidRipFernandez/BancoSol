using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transacciones.Core.DTOs.Transacciones
{
    public class TransaccionResponseDto
    {
        public int Id { get; set; }
        public int CuentaId { get; set; }
        public string TipoTransaccion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoNuevo { get; set; }
        public DateTime FechaTransaccion { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
