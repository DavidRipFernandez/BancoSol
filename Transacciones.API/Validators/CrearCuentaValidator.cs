using FluentValidation;
using Transacciones.Core.DTOs.Cuentas;

namespace Transacciones.API.Validators
{
    public class CrearCuentaValidator : AbstractValidator<CrearCuentaDto>
    {
        public CrearCuentaValidator()
        {
            RuleFor(x => x.NumeroCuenta)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.SaldoInicial)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Titular)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
