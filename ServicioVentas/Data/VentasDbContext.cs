// Archivo: ServicioVentas/Data/VentasDbContext.cs
using Microsoft.EntityFrameworkCore;
using ServicioVentas.Models; // Asegúrate de que el namespace de tus modelos sea correcto

namespace ServicioVentas.Data
{
    public class VentasDbContext : DbContext
    {
        public VentasDbContext(DbContextOptions<VentasDbContext> options)
            : base(options)
        {
        }

        // DbSets para tus modelos Venta y DetalleVenta
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        // DbSets para los nuevos modelos de Cotización y DetalleCotizacion
        public DbSet<Cotizacion> Cotizaciones { get; set; } // Añadido
        public DbSet<DetalleCotizacion> DetallesCotizacion { get; set; } // Añadido

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de precisión para propiedades decimales de Venta y DetalleVenta
            modelBuilder.Entity<DetalleVenta>().Property(d => d.PrecioEnVenta).HasPrecision(18, 2);
            modelBuilder.Entity<Venta>().Property(v => v.TotalVenta).HasPrecision(18, 2);

            // Configuración de precisión para propiedades decimales de Cotizacion y DetalleCotizacion (Añadido)
            modelBuilder.Entity<DetalleCotizacion>().Property(d => d.PrecioEnCotizacion).HasPrecision(18, 2);
            modelBuilder.Entity<Cotizacion>().Property(c => c.TotalCotizacion).HasPrecision(18, 2);


            // Configuración de la relación uno a muchos entre Venta y DetalleVenta
            // Una Venta tiene muchos DetallesVenta, y un DetalleVenta pertenece a una Venta
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)              // Un DetalleVenta tiene una Venta
                .WithMany(v => v.Detalles)         // Una Venta tiene muchos Detalles
                .HasForeignKey(d => d.VentaId)     // La clave foránea es VentaId en DetalleVenta
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina una Venta, se eliminan sus Detalles

            // Configuración de la relación uno a muchos entre Cotizacion y DetalleCotizacion (Añadido)
            // Una Cotizacion tiene muchos DetallesCotizacion, y un DetalleCotizacion pertenece a una Cotizacion
            modelBuilder.Entity<DetalleCotizacion>()
                .HasOne(d => d.Cotizacion)             // Un DetalleCotizacion tiene una Cotizacion
                .WithMany(c => c.Detalles)             // Una Cotizacion tiene muchos Detalles
                .HasForeignKey(d => d.CotizacionId)    // La clave foránea es CotizacionId en DetalleCotizacion
                .OnDelete(DeleteBehavior.Cascade);     // Si se elimina una Cotizacion, se eliminan sus Detalles
        }
    }
}