namespace Sesion_07.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sesion_07.Models;
using Sesion_07.Services;

/// <summary>
/// Subtema 7 y 9: Controlador REST delgado (Thin Controller) con semántica de respuestas HTTP.
/// El controlador NO accede a bases de datos ni calcula reglas; delega a IProductoServicio.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoServicio servicio) : ControllerBase
{
    /// <summary>
    /// GET /api/productos -> Retorna 200 OK con la lista completa.
    /// </summary>
    [HttpGet]
    public ActionResult<List<ProductoItemDto>> ObtenerTodos()
    {
        var productos = servicio.ListarTodos();
        return Ok(productos);
    }

    /// <summary>
    /// GET /api/productos/{id} -> Retorna 200 OK si existe, o 404 NotFound si no se encuentra.
    /// </summary>
    [HttpGet("{id:int}")]
    public ActionResult<ProductoItemDto> ObtenerPorId(int id)
    {
        var producto = servicio.BuscarPorId(id);
        if (producto is null)
            return NotFound(new { mensaje = $"El producto con ID {id} no existe en el catálogo." });

        return Ok(producto);
    }

    /// <summary>
    /// POST /api/productos -> Retorna 201 Created con cabecera Location hacia el recurso recién creado.
    /// </summary>
    [HttpPost]
    public ActionResult<ProductoItemDto> Crear([FromBody] CrearProductoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest(new { mensaje = "El nombre del producto es obligatorio." });

        var nuevo = servicio.Registrar(dto);
        // CreatedAtAction agrega automáticamente la cabecera 'Location: http://localhost:5100/api/productos/{id}'
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevo.Id }, nuevo);
    }

    /// <summary>
    /// DELETE /api/productos/{id} -> Retorna 204 NoContent si se eliminó, o 404 NotFound.
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        bool eliminado = servicio.Eliminar(id);
        if (!eliminado)
            return NotFound(new { mensaje = $"No se pudo eliminar: Producto {id} no existe." });

        return NoContent();
    }

    /// <summary>
    /// GET /api/productos/simular-error-500
    /// [DEMO EN CLASE]: Lanza una excepción imprevista para ver al Middleware RFC 7807 responder con HTTP 500.
    /// </summary>
    [HttpGet("simular-error-500")]
    public IActionResult SimularFallaServidor()
    {
        throw new Exception("Fallo simulado: Desconexión repentina con el motor de base de datos.");
    }
}
