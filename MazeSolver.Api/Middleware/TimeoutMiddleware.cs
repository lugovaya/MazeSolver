using System.Net;

namespace MazeSolver.Api.Middleware
{
    public class TimeoutMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private readonly RequestDelegate _next = next;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("RequestTimeout"));

        public async Task InvokeAsync(HttpContext context)
        {
            using var cts = new CancellationTokenSource(_timeout);
            context.RequestAborted = cts.Token;
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (cts.IsCancellationRequested)
            {
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                await context.Response.WriteAsync("The request timed out.");
            }
        }
    }
}
