namespace MazeSolver.Api.Middleware
{
    public class TimeoutMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private readonly RequestDelegate _next = next;
        // TODO: add RequestTimeout to appsettings.json
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10); // configuration.GetValue<int>("RequestTimeout")

        public async Task InvokeAsync(HttpContext context)
        {
            using var cts = new CancellationTokenSource(_timeout);
            context.RequestAborted = cts.Token;
            await _next(context);
        }
    }
}
