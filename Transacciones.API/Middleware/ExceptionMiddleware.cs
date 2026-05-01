using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using Transacciones.Core.Exceptions;

namespace Transacciones.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, code, message) = ex switch
            {
                CuentaNoEncontradaException => (HttpStatusCode.NotFound, "CUENTA_NO_ENCONTRADA", ex.Message),
                CuentaInactivaException => (HttpStatusCode.BadRequest, "CUENTA_INACTIVA", ex.Message),
                SaldoInsuficienteException => (HttpStatusCode.BadRequest, "SALDO_INSUFICIENTE", ex.Message),
                DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "CONFLICTO_CONCURRENCIA", "La cuenta fue modificada simultáneamente. Intente nuevamente."),
                _ => (HttpStatusCode.InternalServerError, "ERROR_INTERNO", "Ocurrió un error interno en el servidor.")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                success = false,
                error = new { code, message }
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
