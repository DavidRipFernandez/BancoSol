using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Transacciones.Core.Interfaces.Repositories;
using Transacciones.Core.Interfaces.Services;
using Transacciones.Infrastructure.Mappings;
using Transacciones.Infrastructure.Persistence.Contexts;
using Transacciones.Infrastructure.Persistence.Repositories;
using Transacciones.Infrastructure.Services;

namespace Transacciones.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<TransaccionesDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICuentaRepository, CuentaRepository>();
            services.AddScoped<ITransaccionRepository, TransaccionRepository>();
            services.AddScoped<ITransaccionService, TransaccionService>();

            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            return services;
        }
    }
}
