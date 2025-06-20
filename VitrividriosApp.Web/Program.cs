using Microsoft.AspNetCore.Hosting; // Agregado por si acaso
using Microsoft.Extensions.Hosting; // Agregado por si acaso
using System;
using System.Net.Http; // Necesario para HttpClient

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(); // Si es Razor Pages
// builder.Services.AddControllersWithViews(); // Si es MVC

// ** CONFIGURACIÓN DE HTTPCLIENTS PARA LOS MICROSERVICIOS API **
// Configura el HttpClient para ServicioCatalogo
builder.Services.AddHttpClient("ServicioCatalogo", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServicioUrls:Catalogo"] ?? throw new InvalidOperationException("URL de ServicioCatalogo no configurada en appsettings.json."));
});

// Configura el HttpClient para ServicioClientes
builder.Services.AddHttpClient("ServicioClientes", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServicioUrls:Clientes"] ?? throw new InvalidOperationException("URL de ServicioClientes no configurada en appsettings.json."));
});

// Configura el HttpClient para ServicioVentas
builder.Services.AddHttpClient("ServicioVentas", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServicioUrls:Ventas"] ?? throw new InvalidOperationException("URL de ServicioVentas no configurada en appsettings.json."));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages(); // Si es Razor Pages
// app.MapControllerRoute( // Si es MVC
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();