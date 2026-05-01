using AutoMapper;
using Transacciones.Core.DTOs.Cuentas;
using Transacciones.Core.DTOs.Transacciones;
using Transacciones.Core.Entities;

namespace Transacciones.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Cuenta, CuentaResponseDTO>();
            CreateMap<Transaccion, TransaccionResponseDto>();
        }
    }
}
