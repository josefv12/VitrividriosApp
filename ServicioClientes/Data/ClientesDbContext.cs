using Microsoft.EntityFrameworkCore;
using ServicioClientes.Models; // Asegúrate de que el namespace de tus modelos sea correcto

namespace ServicioClientes.Data
{
    public class ClientesDbContext : DbContext
    {
        public ClientesDbContext(DbContextOptions<ClientesDbContext> options)
            : base(options)
        {
        }

        // DbSet para tu modelo Cliente, representa la tabla 'Clientes' en la base de datos
        public DbSet<Cliente> Clientes { get; set; }

        // Puedes añadir configuraciones adicionales del modelo aquí si es necesario
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Aquí podrías añadir configuraciones específicas para el modelo Cliente
            // Por ejemplo, para asegurar la precisión de tipos decimales si el Cliente los tuviera
            // modelBuilder.Entity<Cliente>().Property(c => c.AlgunaPropiedadDecimal).HasPrecision(18, 2);
        }
    }
}