using System.Net;
using System.Text.Json;
using Serilog;

namespace HotelBooking.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            // Pass request to next middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log error
            Log.Error(ex, "Unhandled exception occurred");

            // Prepare response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                message = "Something went wrong",
                error = ex.Message // remove in production if needed
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}