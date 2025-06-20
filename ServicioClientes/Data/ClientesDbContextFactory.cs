using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System; // Agregado para AppDomain
using System.IO;

namespace ServicioClientes.Data
{
    public class ClientesDbContextFactory : IDesignTimeDbContextFactory<ClientesDbContext>
    {
        public ClientesDbContext CreateDbContext(string[] args)
        {
            // Obtiene la ruta base del dominio de la aplicación, que es más fiable
            // para encontrar el appsettings.json durante el tiempo de diseño.
            var basePath = AppContext.BaseDirectory; // Usa AppContext.BaseDirectory para .NET Core 3.0+

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath) // Establece el directorio base usando AppContext.BaseDirectory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Asegura que el archivo se requiere y se puede recargar
                .Build();

            var builder = new DbContextOptionsBuilder<ClientesDbContext>();
            var connectionString = configuration.GetConnectionString("ClientesDbConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                // Lanza una excepción si la cadena de conexión no se encuentra,
                // lo que es útil para depurar si el archivo no se carga o el nombre es incorrecto.
                throw new InvalidOperationException("No se encontró la cadena de conexión 'ClientesDbConnection' en appsettings.json.");
            }

            builder.UseSqlServer(connectionString);

            return new ClientesDbContext(builder.Options);
        }
    }
}