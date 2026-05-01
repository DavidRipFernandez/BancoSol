using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transacciones.Core.Entities;

namespace Transacciones.Infrastructure.Persistence.Contexts
{
    public class TransaccionesDbContext : DbContext
    {
        public TransaccionesDbContext(DbContextOptions<TransaccionesDbContext> options)
        : base(options) { }

        public DbSet<Cuenta> Cuentas => Set<Cuenta>();
        public DbSet<Transaccion> Transacciones => Set<Transaccion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cuenta>(e =>
            {
                e.HasKey(c => c.Id);
                e.Property(c => c.NumeroCuenta)
                 .HasMaxLength(20)
                 .IsRequired();
                e.HasIndex(c => c.NumeroCuenta)
                 .IsUnique();
                e.Property(c => c.Titular)
                 .HasMaxLength(100)
                 .IsRequired();
                e.Property(c => c.Saldo)
                 .HasColumnType("decimal(18,2)");
                e.Property(c => c.RowVersion)
                 .IsRowVersion();  //control de concurrencia
            });

            modelBuilder.Entity<Transaccion>(e =>
            {
                e.HasKey(t => t.Id);
                e.Property(t => t.TipoTransaccion)
                 .HasMaxLength(10)
                 .IsRequired();
                e.Property(t => t.Monto)
                 .HasColumnType("decimal(18,2)");
                e.Property(t => t.SaldoAnterior)
                 .HasColumnType("decimal(18,2)");
                e.Property(t => t.SaldoNuevo)
                 .HasColumnType("decimal(18,2)");
                e.Property(t => t.Descripcion)
                 .HasMaxLength(255);

                e.HasOne(t => t.Cuenta)
                 .WithMany(c => c.Transacciones)
                 .HasForeignKey(t => t.CuentaId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(t => t.CuentaId);
                e.HasIndex(t => t.FechaTransaccion);
            });
        }
    }
}
