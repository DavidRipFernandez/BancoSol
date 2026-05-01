using FluentValidation;
using Transacciones.Core.DTOs.Transacciones;

namespace Transacciones.API.Validators
{
    public class RealizarTransaccionValidator : AbstractValidator<RealizarTransaccionDto>
    {
        public RealizarTransaccionValidator()
        {
            RuleFor(x => x.Monto)
                .GreaterThan(0);

            RuleFor(x => x.Descripcion)
                .NotEmpty()
                .MaximumLength(255);
        }
    }
}
