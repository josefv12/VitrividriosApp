using Microsoft.EntityFrameworkCore;
using ServicioVentas.Data; // Aseg�rate de tener este using
using ServicioVentas.Strategies; // Aseg�rate de agregar este using para las estrategias
using System; // Agregado para Uri
using System.Net.Http; // Necesario para HttpClient
using System.Text.Json.Serialization; // <--- �NUEVO USING NECESARIO!
using System.Text.Json; // <--- �NUEVO USING NECESARIO para JsonNamingPolicy!

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options => // <--- SECCI�N A�ADIDA PARA MANEJAR SERIALIZACI�N JSON
    {
        // Ignora los ciclos de referencia al serializar objetos.
        // Es crucial para evitar errores de ciclo infinito en relaciones padre-hijo (ej. Venta -> Detalles -> Venta).
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Opcional: para que las propiedades en JSON est�n en camelCase (primera letra min�scula),
        // lo cual es una convenci�n com�n en APIs REST.
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuraci�n del DbContext para ServicioVentas
builder.Services.AddDbContext<VentasDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VentasDbConnection")));

// ** CONFIGURACI�N DE HTTP CLIENTS PARA OTROS SERVICIOS **
// Configura el HttpClient para ServicioCatalogo
builder.Services.AddHttpClient("ServicioCatalogo", client =>
{
    // Aseg�rate de que esta URL coincida con la URL base de tu ServicioCatalogo
    // (por ejemplo, https://localhost:7001/ si esa es la que usa al ejecutarlo)
    // Puedes obtenerla de las propiedades del proyecto ServicioCatalogo (Debug -> Open debug launch profiles UI).
    client.BaseAddress = new Uri(builder.Configuration["ServicioUrls:Catalogo"] ?? throw new InvalidOperationException("URL de ServicioCatalogo no configurada."));
});

// Configura el HttpClient para ServicioClientes
builder.Services.AddHttpClient("ServicioClientes", client =>
{
    // Aseg�rate de que esta URL coincida con la URL base de tu ServicioClientes
    // (por ejemplo, https://localhost:7002/ si esa es la que usa al ejecutarlo)
    // Puedes obtenerla de las propiedades del proyecto ServicioClientes (Debug -> Open debug launch profiles UI).
    client.BaseAddress = new Uri(builder.Configuration["ServicioUrls:Clientes"] ?? throw new InvalidOperationException("URL de ServicioClientes no configurada."));
});

// ** SECCI�N PARA REGISTRAR LAS ESTRATEGIAS DE PRECIO **
// Registra las implementaciones de ICalculoPrecioStrategy como servicios transitorios.
// Esto significa que se crear� una nueva instancia cada vez que se soliciten.
builder.Services.AddTransient<EstrategiaPrecioPublico>();
builder.Services.AddTransient<EstrategiaPrecioMayorista>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();