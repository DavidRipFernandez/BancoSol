using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Transacciones.Core.DTOs.Transacciones;
using Transacciones.Core.Entities;
using Transacciones.Core.Exceptions;
using Transacciones.Core.Interfaces.Repositories;
using Transacciones.Infrastructure.Persistence.Contexts;
using Transacciones.Infrastructure.Services;

namespace Transacciones.Tests.Services
{
    public class TransaccionServiceTests
    {
        private static TransaccionesDbContext CrearContextoInMemory()
        {
            var options = new DbContextOptionsBuilder<TransaccionesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new TransaccionesDbContext(options);
        }

        private static IMapper CrearMapperReal()
        {
            var mockMapper = new Mock<IMapper>();

            mockMapper
                .Setup(m => m.Map<TransaccionResponseDto>(It.IsAny<Transaccion>()))
                .Returns((Transaccion t) => new TransaccionResponseDto
                {
                    Id = t.Id,
                    CuentaId = t.CuentaId,
                    TipoTransaccion = t.TipoTransaccion,
                    Monto = t.Monto,
                    SaldoAnterior = t.SaldoAnterior,
                    SaldoNuevo = t.SaldoNuevo,
                    FechaTransaccion = t.FechaTransaccion,
                    Descripcion = t.Descripcion
                });

            return mockMapper.Object;
        }

        [Fact]
        public async Task RealizarRetiro_ConSaldoInsuficiente_DeberiaLanzarSaldoInsuficienteException()
        {
            // Arrange
            var cuenta = new Cuenta { Id = 1, Saldo = 100, Activa = true };
            var mockCuentaRepo = new Mock<ICuentaRepository>();
            mockCuentaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cuenta);

            using var context = CrearContextoInMemory();
            var service = new TransaccionService(
                mockCuentaRepo.Object,
                new Mock<ITransaccionRepository>().Object,
                context,
                CrearMapperReal());

            var dto = new RealizarTransaccionDto { Monto = 200, Descripcion = "Retiro excesivo" };

            // Act & Assert
            await Assert.ThrowsAsync<SaldoInsuficienteException>(
                () => service.RealizarRetiro(1, dto));
        }

        [Fact]
        public async Task RealizarRetiro_CuentaInactiva_DeberiaLanzarCuentaInactivaException()
        {
            // Arrange
            var cuenta = new Cuenta { Id = 2, Saldo = 500, Activa = false };
            var mockCuentaRepo = new Mock<ICuentaRepository>();
            mockCuentaRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(cuenta);

            using var context = CrearContextoInMemory();
            var service = new TransaccionService(
                mockCuentaRepo.Object,
                new Mock<ITransaccionRepository>().Object,
                context,
                CrearMapperReal());

            var dto = new RealizarTransaccionDto { Monto = 100, Descripcion = "Retiro cuenta inactiva" };

            // Act & Assert
            await Assert.ThrowsAsync<CuentaInactivaException>(
                () => service.RealizarRetiro(2, dto));
        }

        [Fact]
        public async Task RealizarAbono_CuentaActiva_DeberiaActualizarSaldo()
        {
            // Arrange
            var cuenta = new Cuenta { Id = 3, Saldo = 100, Activa = true };
            var mockCuentaRepo = new Mock<ICuentaRepository>();
            mockCuentaRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(cuenta);

            var mockTransaccionRepo = new Mock<ITransaccionRepository>();
            mockTransaccionRepo
                .Setup(r => r.CreateAsync(It.IsAny<Transaccion>()))
                .ReturnsAsync((Transaccion t) => t);

            using var context = CrearContextoInMemory();
            var service = new TransaccionService(
                mockCuentaRepo.Object,
                mockTransaccionRepo.Object,
                context,
                CrearMapperReal());

            var dto = new RealizarTransaccionDto { Monto = 50, Descripcion = "Abono de prueba" };

            // Act
            var result = await service.RealizarAbono(3, dto);

            // Assert
            Assert.Equal(150, result.SaldoNuevo);
            Assert.Equal(100, result.SaldoAnterior);
            Assert.Equal("ABONO", result.TipoTransaccion);
        }

        [Fact]
        public async Task RealizarRetiro_CuentaNoExiste_DeberiaLanzarCuentaNoEncontradaException()
        {
            // Arrange
            var mockCuentaRepo = new Mock<ICuentaRepository>();
            mockCuentaRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cuenta?)null);

            using var context = CrearContextoInMemory();
            var service = new TransaccionService(
                mockCuentaRepo.Object,
                new Mock<ITransaccionRepository>().Object,
                context,
                CrearMapperReal());

            var dto = new RealizarTransaccionDto { Monto = 100, Descripcion = "Cuenta inexistente" };

            // Act & Assert
            await Assert.ThrowsAsync<CuentaNoEncontradaException>(
                () => service.RealizarRetiro(99, dto));
        }
    }
}
