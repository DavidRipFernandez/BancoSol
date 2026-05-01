using AutoMapper;
using Serilog;
using Transacciones.Core.DTOs.Transacciones;
using Transacciones.Core.Entities;
using Transacciones.Core.Exceptions;
using Transacciones.Core.Interfaces.Repositories;
using Transacciones.Core.Interfaces.Services;
using Transacciones.Infrastructure.Persistence.Contexts;

namespace Transacciones.Infrastructure.Services
{
    public class TransaccionService : ITransaccionService
    {
        private readonly ICuentaRepository _cuentaRepository;
        private readonly ITransaccionRepository _transaccionRepository;
        private readonly TransaccionesDbContext _context;
        private readonly IMapper _mapper;

        public TransaccionService(
            ICuentaRepository cuentaRepository,
            ITransaccionRepository transaccionRepository,
            TransaccionesDbContext context,
            IMapper mapper)
        {
            _cuentaRepository = cuentaRepository;
            _transaccionRepository = transaccionRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<TransaccionResponseDto> RealizarAbono(int cuentaId, RealizarTransaccionDto dto)
        {
            Log.Information("Iniciando abono de {Monto} en cuenta {CuentaId}", dto.Monto, cuentaId);

            var cuenta = await _cuentaRepository.GetByIdAsync(cuentaId);
            if (cuenta == null) throw new CuentaNoEncontradaException(cuentaId);

            if (!cuenta.Activa)
            {
                Log.Warning("Cuenta {CuentaId} inactiva, abono rechazado", cuentaId);
                throw new CuentaInactivaException(cuentaId);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var saldoAnterior = cuenta.Saldo;
                cuenta.Saldo += dto.Monto;

                var transaccion = new Transaccion
                {
                    CuentaId = cuentaId,
                    TipoTransaccion = "ABONO",
                    Monto = dto.Monto,
                    Descripcion = dto.Descripcion,
                    SaldoAnterior = saldoAnterior,
                    SaldoNuevo = cuenta.Saldo
                };

                await _transaccionRepository.CreateAsync(transaccion);
                await transaction.CommitAsync();

                return _mapper.Map<TransaccionResponseDto>(transaccion);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en abono de cuenta {CuentaId}, ejecutando rollback", cuentaId);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<TransaccionResponseDto> RealizarRetiro(int cuentaId, RealizarTransaccionDto dto)
        {
            Log.Information("Iniciando retiro de {Monto} de cuenta {CuentaId}", dto.Monto, cuentaId);

            var cuenta = await _cuentaRepository.GetByIdAsync(cuentaId);
            if (cuenta == null) throw new CuentaNoEncontradaException(cuentaId);

            if (!cuenta.Activa)
            {
                Log.Warning("Cuenta {CuentaId} inactiva, retiro rechazado", cuentaId);
                throw new CuentaInactivaException(cuentaId);
            }

            if (cuenta.Saldo < dto.Monto)
            {
                Log.Warning("Saldo insuficiente en cuenta {CuentaId}: disponible {Disponible}, solicitado {Solicitado}",
                    cuentaId, cuenta.Saldo, dto.Monto);
                throw new SaldoInsuficienteException(cuenta.Saldo, dto.Monto);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var saldoAnterior = cuenta.Saldo;
                cuenta.Saldo -= dto.Monto;

                var transaccion = new Transaccion
                {
                    CuentaId = cuentaId,
                    TipoTransaccion = "RETIRO",
                    Monto = dto.Monto,
                    Descripcion = dto.Descripcion,
                    SaldoAnterior = saldoAnterior,
                    SaldoNuevo = cuenta.Saldo
                };

                await _transaccionRepository.CreateAsync(transaccion);
                await transaction.CommitAsync();

                return _mapper.Map<TransaccionResponseDto>(transaccion);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en retiro de cuenta {CuentaId}, ejecutando rollback", cuentaId);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<TransaccionResponseDto>> ObtenerHistorial(int cuentaId)
        {
            var cuenta = await _cuentaRepository.GetByIdAsync(cuentaId);
            if (cuenta == null) throw new CuentaNoEncontradaException(cuentaId);

            var transacciones = await _transaccionRepository.GetByCuentaIdAsync(cuentaId);
            return _mapper.Map<IEnumerable<TransaccionResponseDto>>(transacciones);
        }
    }
}
