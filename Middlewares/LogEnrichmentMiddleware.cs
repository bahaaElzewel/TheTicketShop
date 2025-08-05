using Serilog.Context;
using System.Diagnostics;

namespace TheTicketShop.Middlewares;
public class LogEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public LogEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        Activity? activity = Activity.Current ?? new Activity("IncomingRequest").Start();

        // RequestId: either extracted from gateway or generated
        string requestId = context.Request.Headers["X-Request-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();

        // Push all logging properties
        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("TraceId", activity?.TraceId.ToString()))
        using (LogContext.PushProperty("SpanId", activity?.SpanId.ToString()))
        using (LogContext.PushProperty("ParentSpanId", activity?.ParentSpanId.ToString()))
        using (LogContext.PushProperty("ServiceName", "TheTicketsShop"))
        {
            await _next(context);
        }
    }
}
