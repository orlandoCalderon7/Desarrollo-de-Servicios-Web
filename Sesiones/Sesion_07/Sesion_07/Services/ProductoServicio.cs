namespace Sesion_07.Services;

using System.Collections.Generic;
using System.Linq;
using Sesion_07.Models;

public interface IProductoServicio
{
    List<ProductoItemDto> ListarTodos();
    ProductoItemDto? BuscarPorId(int id);
    ProductoItemDto Registrar(CrearProductoDto dto);
    bool Eliminar(int id);
}

/// <summary>
/// Simulación de persistencia en memoria (List) para la Web API sin requerir motor de base de datos externo.
/// </summary>
public class ProductoServicioEnMemoria : IProductoServicio
{
    private readonly List<ProductoItemDto> _productos = new()
    {
        new(101, "Laptop Gamer Core i7", "Computación", 3500.00m),
        new(102, "Monitor 27 Pulgadas 144Hz", "Periféricos", 850.00m),
        new(103, "Mouse Ergonómico Inalámbrico", "Periféricos", 120.00m)
    };

    public List<ProductoItemDto> ListarTodos() => _productos.ToList();

    public ProductoItemDto? BuscarPorId(int id) => _productos.FirstOrDefault(p => p.Id == id);

    public ProductoItemDto Registrar(CrearProductoDto dto)
    {
        // Regla de negocio simulada
        if (dto.Precio <= 0)
            throw new InvalidOperationException("El precio del producto debe ser mayor a cero.");

        int nuevoId = _productos.Any() ? _productos.Max(p => p.Id) + 1 : 101;
        var nuevo = new ProductoItemDto(nuevoId, dto.Nombre, dto.Categoria, dto.Precio);
        _productos.Add(nuevo);
        return nuevo;
    }

    public bool Eliminar(int id)
    {
        var item = BuscarPorId(id);
        if (item is null) return false;
        return _productos.Remove(item);
    }
}
