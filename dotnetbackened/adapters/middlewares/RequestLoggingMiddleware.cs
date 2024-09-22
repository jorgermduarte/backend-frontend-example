using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace dotnetbackened.adapters.middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Start logging
            var stopwatch = Stopwatch.StartNew();

            // Log the incoming request
            _logger.LogInformation("Incoming request: {method} {url}", context.Request.Method, context.Request.Path);

            // Call the next middleware in the pipeline
            await _next(context);

            // Log the response status code
            stopwatch.Stop();
            _logger.LogInformation("Response status code: {statusCode} | Duration: {duration} ms",
                context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
    }

}
