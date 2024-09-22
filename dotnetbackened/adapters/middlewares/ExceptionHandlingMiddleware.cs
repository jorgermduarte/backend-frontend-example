using System.Net;
using System.Text.Json;

namespace dotnetbackened.adapters.middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError($"An error occurred: {ex}");
                await HandleExceptionAsync(context, ex);
            }

            // Handle non-exception errors (e.g., 404)
            if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 600 && !context.Response.HasStarted)
            {
                await HandleErrorAsync(context);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = context.Response;
            var errorResponse = new ErrorResponse
            {
                Message = exception.Message,
                Code = "INTERNAL_SERVER_ERROR"
            };

            switch (exception)
            {
                case ApplicationException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Code = "APPLICATION_ERROR";
                    break;
                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Code = "NOT_FOUND";
                    break;
                case UnauthorizedAccessException ex:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Code = "UNAUTHORIZED";
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            errorResponse.StatusCode = response.StatusCode;

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }

        private async Task HandleErrorAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = GetDefaultMessageForStatusCode(context.Response.StatusCode),
                Code = GetDefaultCodeForStatusCode(context.Response.StatusCode)
            };

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                405 => "Method Not Allowed",
                500 => "Internal Server Error",
                _ => "An error occurred"
            };
        }

        private string GetDefaultCodeForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "BAD_REQUEST",
                401 => "UNAUTHORIZED",
                403 => "FORBIDDEN",
                404 => "NOT_FOUND",
                405 => "METHOD_NOT_ALLOWED",
                500 => "INTERNAL_SERVER_ERROR",
                _ => "ERROR"
            };
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public int StatusCode { get; set; }
    }
}
