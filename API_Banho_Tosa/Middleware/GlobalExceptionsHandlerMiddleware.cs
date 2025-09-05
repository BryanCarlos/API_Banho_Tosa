using System.Net;
using System.Text.Json;

namespace API_Banho_Tosa.Middleware
{
    public class GlobalExceptionsHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionsHandlerMiddleware(RequestDelegate next)
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
                await HandleExceptionsAsync(context, ex);
            }
        }

        private Task HandleExceptionsAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case ArgumentNullException:
                case ArgumentOutOfRangeException:
                case ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                break;

                case KeyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = exception.Message;
                break;

                case InvalidOperationException:
                    status = HttpStatusCode.Conflict;
                    message = exception.Message;
                break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    message = exception.Message;
                    // TODO: Logar a excecao
                break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;


            string result = JsonSerializer.Serialize(new { error = message });
            return context.Response.WriteAsync(result);
        }
    }
}
