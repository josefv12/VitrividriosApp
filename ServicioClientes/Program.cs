using Microsoft.EntityFrameworkCore; // Aseg�rate de tener este using
using ServicioClientes.Data; // Aseg�rate de tener este using

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ** A�ADE ESTA SECCI�N PARA EL DBContext **
builder.Services.AddDbContext<ClientesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ClientesDbConnection")));

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