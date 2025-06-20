using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ServicioVentas.Data
{
    public class VentasDbContextFactory : IDesignTimeDbContextFactory<VentasDbContext>
    {
        public VentasDbContext CreateDbContext(string[] args)
        {
            var basePath = AppContext.BaseDirectory;

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var builder = new DbContextOptionsBuilder<VentasDbContext>();
            var connectionString = configuration.GetConnectionString("VentasDbConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("No se encontró la cadena de conexión 'VentasDbConnection' en appsettings.json.");
            }

            builder.UseSqlServer(connectionString);

            return new VentasDbContext(builder.Options);
        }
    }
}