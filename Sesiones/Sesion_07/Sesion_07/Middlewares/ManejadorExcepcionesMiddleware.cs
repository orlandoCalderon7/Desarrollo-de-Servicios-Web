namespace Sesion_07.Middlewares;

using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Subtema 8: Middleware global de excepciones bajo el estándar internacional RFC 7807 (ProblemDetails).
/// Captura cualquier error no controlado ocurrido en los controladores y devuelve un JSON uniforme
/// sin exponer jamás el Stack Trace en producción.
/// </summary>
public class ManejadorExcepcionesMiddleware(RequestDelegate next, ILogger<ManejadorExcepcionesMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Excepción capturada en {Path}: {Message}", context.Request.Path, ex.Message);
            await ManejarExcepcionAsync(context, ex);
        }
    }

    private static Task ManejarExcepcionAsync(HttpContext context, Exception excepcion)
    {
        context.Response.ContentType = "application/problem+json";

        var problema = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807",
            Instance = context.Request.Path
        };

        if (excepcion is InvalidOperationException)
        {
            // 400 Bad Request: Error atribuible a reglas de negocio o datos del cliente
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            problema.Status = StatusCodes.Status400BadRequest;
            problema.Title = "Operación de Negocio Inválida";
            problema.Detail = excepcion.Message;
        }
        else
        {
            // 500 Internal Server Error: Falla inesperada de infraestructura/código
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            problema.Status = StatusCodes.Status500InternalServerError;
            problema.Title = "Error Interno en el Servidor";
            problema.Detail = "Ha ocurrido un error inesperado al procesar su solicitud. Por favor, intente más tarde.";
        }

        var opciones = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(problema, opciones));
    }
}
