// Middleware/ExceptionMiddleware.cs
// This middleware catches all unhandled exceptions and returns a clean JSON error response
// Without this, .NET would return a raw 500 HTML error page which is bad for APIs

using System.Net;
using System.Text.Json;

namespace HotelBooking.Middleware
{
    public class ExceptionMiddleware
    {
        // _next represents the next piece of middleware in the pipeline
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Try to process the request normally
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the full error for debugging
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                // Return a clean JSON error to the client
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Choose HTTP status code based on exception type
            var statusCode = exception switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,         // 404
                UnauthorizedAccessException => HttpStatusCode.Forbidden, // 403
                ArgumentException => HttpStatusCode.BadRequest,          // 400
                InvalidOperationException => HttpStatusCode.BadRequest,  // 400
                _ => HttpStatusCode.InternalServerError                  // 500
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = (int)statusCode,
                Message = exception.Message
            };

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}