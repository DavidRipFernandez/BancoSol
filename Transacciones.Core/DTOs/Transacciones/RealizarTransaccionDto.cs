using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transacciones.Core.DTOs.Transacciones
{
    public class RealizarTransaccionDto
    {
        public decimal Monto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
