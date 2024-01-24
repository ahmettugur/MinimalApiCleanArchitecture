using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.OpenTelemetry.Middlewares;

public class OpenTelemetryTraceIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<OpenTelemetryTraceIdMiddleware>>();
        var traceId = Activity.Current?.TraceId.ToString();
        using (logger.BeginScope("{@OpenTelemetryTraceId}", traceId))
        {
            await next(context);
        }
    }
}