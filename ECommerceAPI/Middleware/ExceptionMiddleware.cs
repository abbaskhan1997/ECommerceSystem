using System.Text.Json;

namespace ECommerceAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware (RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync (HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Actual error Visual Studio ke Output mein dekho
                Console.WriteLine("ERROR: " + ex);
                Console.WriteLine("INNER ERROR: " + ex.InnerException?.Message);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "Something went wrong",
                    error = ex.InnerException?.Message ?? ex.Message
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
            }
        }
    }
}