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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de precisión para propiedades decimales
            modelBuilder.Entity<DetalleVenta>().Property(d => d.PrecioEnVenta).HasPrecision(18, 2);
            modelBuilder.Entity<Venta>().Property(v => v.TotalVenta).HasPrecision(18, 2);

            // Configuración de la relación uno a muchos entre Venta y DetalleVenta
            // Una Venta tiene muchos DetallesVenta, y un DetalleVenta pertenece a una Venta
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)           // Un DetalleVenta tiene una Venta
                .WithMany(v => v.Detalles)      // Una Venta tiene muchos Detalles
                .HasForeignKey(d => d.VentaId)  // La clave foránea es VentaId en DetalleVenta
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina una Venta, se eliminan sus Detalles
        }
    }
}