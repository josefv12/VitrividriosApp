using Microsoft.EntityFrameworkCore;
using ServicioCatalogo.Models; // Asegúrate de que el namespace de tus modelos sea correcto

namespace ServicioCatalogo.Data
{
    public class CatalogoDbContext : DbContext
    {
        public CatalogoDbContext(DbContextOptions<CatalogoDbContext> options)
            : base(options)
        {
        }

        // DbSet para tu modelo Producto, representa la tabla 'Productos' en la base de datos
        public DbSet<Producto> Productos { get; set; }

        // Puedes añadir configuraciones adicionales del modelo aquí si es necesario
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ** CONFIGURACIÓN PARA LAS PROPIEDADES DECIMAL **
            // Esto le dice a EF Core que use un tipo de columna decimal(18,2) en SQL Server
            // para PrecioUnitario y PrecioMayorista.
            // 18 es la precisión total (número de dígitos) y 2 es la escala (número de dígitos después del punto decimal).
            // Puedes ajustar estos valores si tus precios requieren mayor precisión (ej. 18,4).
            modelBuilder.Entity<Producto>().Property(p => p.PrecioUnitario).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.PrecioMayorista).HasPrecision(18, 2);

            // Ejemplo: Configurar el nombre de la tabla si no quieres que sea 'Productos'
            // modelBuilder.Entity<Producto>().ToTable("ProductosCatalogo");
        }
    }
}