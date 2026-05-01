using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transacciones.Core.Entities
{
    public class Cuenta
    {
        public int Id { get; set; }
        public string NumeroCuenta { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public string Titular { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool Activa { get; set; } = true;

        //concurrencia
        [System.ComponentModel.DataAnnotations.Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        public ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();

    }
}
