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
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "Something went wrong",
                    error = ex.Message
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
            }
        }
        }
}
