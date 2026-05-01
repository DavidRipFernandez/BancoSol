using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Transacciones.Core.DTOs.Transacciones;
using Transacciones.Core.Interfaces.Services;

namespace Transacciones.API.Controllers
{
    public record RealizarTransaccionRequest(int CuentaId, decimal Monto, string Descripcion);

    [ApiController]
    [Route("api/transacciones")]
    public class TransaccionesController : ControllerBase
    {
        private readonly ITransaccionService _transaccionService;
        private readonly IMemoryCache _cache;
        private readonly IValidator<RealizarTransaccionDto> _validator;

        public TransaccionesController(
            ITransaccionService transaccionService,
            IMemoryCache cache,
            IValidator<RealizarTransaccionDto> validator)
        {
            _transaccionService = transaccionService;
            _cache = cache;
            _validator = validator;
        }

        [HttpPost("abono")]
        public async Task<IActionResult> Abono([FromBody] RealizarTransaccionRequest request)
        {
            var dto = new RealizarTransaccionDto { Monto = request.Monto, Descripcion = request.Descripcion };
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var result = await _transaccionService.RealizarAbono(request.CuentaId, dto);
            _cache.Remove($"cuenta_{request.CuentaId}");
            return Ok(result);
        }

        [HttpPost("retiro")]
        public async Task<IActionResult> Retiro([FromBody] RealizarTransaccionRequest request)
        {
            var dto = new RealizarTransaccionDto { Monto = request.Monto, Descripcion = request.Descripcion };
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var result = await _transaccionService.RealizarRetiro(request.CuentaId, dto);
            _cache.Remove($"cuenta_{request.CuentaId}");
            return Ok(result);
        }

        [HttpGet("cuenta/{id}")]
        public async Task<IActionResult> ObtenerHistorial(int id)
        {
            var result = await _transaccionService.ObtenerHistorial(id);
            return Ok(result);
        }
    }
}
