using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transacciones.Core.Entities
{
    public class Transaccion
    {
        public int Id { get; set; }
        public int CuentaId { get; set; }
        public string TipoTransaccion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime FechaTransaccion { get; set; } = DateTime.UtcNow;
        public string Descripcion { get; set; } = string.Empty;
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoNuevo {  get; set; }
        public Cuenta Cuenta { get; set; } = null!;
    }
}
