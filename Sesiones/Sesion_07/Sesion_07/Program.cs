using Scalar.AspNetCore;
using Sesion_07.Middlewares;
using Sesion_07.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Contenedor de Inversión de Control (IoC)
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IProductoServicio, ProductoServicioEnMemoria>();

var app = builder.Build();

// 2. Pipeline de Middlewares
app.UseMiddleware<ManejadorExcepcionesMiddleware>();

// 3. Documentación Interactiva OpenAPI + Scalar (/scalar/v1)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Sesión 07 - Catálogo de Productos API")
               .WithTheme(ScalarTheme.Purple);
    });
}

app.MapControllers();

app.Run();
