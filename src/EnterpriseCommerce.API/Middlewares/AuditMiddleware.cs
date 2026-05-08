namespace EnterpriseCommerce.API.Middlewares
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(
            RequestDelegate next,
            ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var user =
                context.User?.Identity?.Name ?? "Anonymous";

            var endpoint =
                context.Request.Path;

            var ip =
                context.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation(
                "User: {User} | Endpoint: {Endpoint} | IP: {IP}",
                user,
                endpoint,
                ip);

            await _next(context);
        }
    }
}
