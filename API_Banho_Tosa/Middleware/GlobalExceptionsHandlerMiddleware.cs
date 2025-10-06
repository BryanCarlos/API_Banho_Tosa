using API_Banho_Tosa.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;

namespace API_Banho_Tosa.Middleware
{
    public class GlobalExceptionsHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionsHandlerMiddleware> _logger;

        public GlobalExceptionsHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionsHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
            var (status, message) = exception switch
            {
                ArgumentException or ArgumentNullException or ArgumentOutOfRangeException =>
                    (HttpStatusCode.BadRequest, exception.Message),

                KeyNotFoundException =>
                    (HttpStatusCode.NotFound, exception.Message),

                InvalidOperationException or UserAlreadyExistsException or PetSizeAlreadyExistsException =>
                    (HttpStatusCode.Conflict, exception.Message),

                UnauthorizedAccessException =>
                    (HttpStatusCode.Unauthorized, exception.Message),

                ConfigurationException =>
                    (HttpStatusCode.Unauthorized, exception.Message),

                _ =>
                    (HttpStatusCode.InternalServerError, "An unexpected server error occurred.")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            if (status == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(
                    exception, 
                    "Unhandled error resulted in InternalServerError. Original message: {ErrorMessage}", 
                    exception.Message
                );
            }
            else
            {
                _logger.LogInformation(
                    "Handled exception resulted in status { StatusCode}. Message: { ErrorMessage}",
                    (int)status,
                    exception.Message
                );
            }

            string result = JsonSerializer.Serialize(new { error = message });
            return context.Response.WriteAsync(result);
        }
    }
}
