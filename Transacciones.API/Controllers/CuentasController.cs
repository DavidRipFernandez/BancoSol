using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Transacciones.Core.DTOs.Cuentas;
using Transacciones.Core.Entities;
using Transacciones.Core.Exceptions;
using Transacciones.Core.Interfaces.Repositories;

namespace Transacciones.API.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly ICuentaRepository _cuentaRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IValidator<CrearCuentaDto> _validator;

        public CuentasController(
            ICuentaRepository cuentaRepository,
            IMapper mapper,
            IMemoryCache cache,
            IValidator<CrearCuentaDto> validator)
        {
            _cuentaRepository = cuentaRepository;
            _mapper = mapper;
            _cache = cache;
            _validator = validator;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearCuentaDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var cuenta = new Cuenta
            {
                NumeroCuenta = dto.NumeroCuenta,
                Saldo = dto.SaldoInicial,
                Titular = dto.Titular
            };

            await _cuentaRepository.CreateAsync(cuenta);
            var response = _mapper.Map<CuentaResponseDTO>(cuenta);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = cuenta.Id }, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var cacheKey = $"cuenta_{id}";

            if (!_cache.TryGetValue(cacheKey, out CuentaResponseDTO? response))
            {
                var cuenta = await _cuentaRepository.GetByIdAsync(id);
                if (cuenta == null) throw new CuentaNoEncontradaException(id);

                response = _mapper.Map<CuentaResponseDTO>(cuenta);
                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
            }

            return Ok(response);
        }
    }
}
