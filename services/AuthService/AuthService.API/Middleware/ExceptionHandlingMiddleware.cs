using System.Net;
using System.Text.Json;

namespace AuthService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            } 
            catch (InvalidOperationException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
        }

        private static Task HandleExceptionAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = (int)statusCode,
                message,
            };

            var json = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(json);
        }
    }
}
