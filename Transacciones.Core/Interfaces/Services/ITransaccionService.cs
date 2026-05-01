using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transacciones.Core.DTOs.Transacciones;

namespace Transacciones.Core.Interfaces.Services
{
    public interface ITransaccionService
    {
        Task<TransaccionResponseDto> RealizarAbono(int cuentaId, RealizarTransaccionDto dto);
        Task<TransaccionResponseDto> RealizarRetiro(int cuentaId, RealizarTransaccionDto dto);
        Task<IEnumerable<TransaccionResponseDto>> ObtenerHistorial(int cuentaId);
    }
}
