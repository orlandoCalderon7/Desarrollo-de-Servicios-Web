namespace Sesion_07.Models;

/// <summary>
/// DTO de respuesta para representar un producto en formato JSON.
/// </summary>
public record ProductoItemDto(int Id, string Nombre, string Categoria, decimal Precio);

/// <summary>
/// DTO de entrada para registrar un producto desde el cuerpo (body) de la petición HTTP POST.
/// </summary>
public record CrearProductoDto(string Nombre, string Categoria, decimal Precio);
