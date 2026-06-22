using GatherUp.core.Exceptions;
using System.Text.Json;

namespace GatherUp.API.Middleware
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
            catch (EntityNotFoundException ex)
            {
                await WriteResponse(context, 404, ex.Message);
            }
            catch (InvalidEventDataException ex)
            {
                await WriteResponse(context, 400, ex.Message);
            }
            catch (DuplicateUserException ex)
            {
                await WriteResponse(context, 409, ex.Message);
            }
            catch (AccessDeniedException ex)
            {
                await WriteResponse(context, 401, ex.Message);
            }
            catch (ImmutableReceiptException ex)
            {
                await WriteResponse(context, 400, ex.Message);
            }
            catch (Exception ex)
            {
                await WriteResponse(context, 500, $"Internal server error: {ex.Message}");
            }
        }

        private static async Task WriteResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
        }
    }
}
