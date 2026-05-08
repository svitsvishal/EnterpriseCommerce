using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EnterpriseCommerce.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>>
        _logger;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation(
            "Handling Request: {RequestName} {@Request}",
            requestName,
            JsonSerializer.Serialize(request));

        var response = await next();

        _logger.LogInformation(
            "Completed Request: {RequestName}",
            requestName);

        return response;
    }
}