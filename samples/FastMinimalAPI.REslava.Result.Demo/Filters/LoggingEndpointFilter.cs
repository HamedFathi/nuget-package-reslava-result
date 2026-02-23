namespace FastMinimalAPI.REslava.Result.Demo.Filters;

/// <summary>
/// Simple endpoint filter that logs request and response info to the console.
/// Used in SmartCatalogController to demonstrate [SmartFilter] endpoint filter registration.
/// </summary>
public class LoggingEndpointFilter : IEndpointFilter
{
    private readonly ILogger<LoggingEndpointFilter> _logger;

    public LoggingEndpointFilter(ILogger<LoggingEndpointFilter> logger) => _logger = logger;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        _logger.LogInformation("[SmartFilter] → {Method} {Path}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path);

        var result = await next(context);

        _logger.LogInformation("[SmartFilter] ← {StatusCode}",
            context.HttpContext.Response.StatusCode);

        return result;
    }
}
