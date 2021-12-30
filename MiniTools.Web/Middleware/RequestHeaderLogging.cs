using Microsoft.Net.Http.Headers;
using Serilog.Context;

namespace MiniTools.Web.Middleware;


public class RequestHeaderLogging
{
    private readonly RequestDelegate next;

    public RequestHeaderLogging(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string traceIdRelation = context.TraceIdentifier;

        if (context.Request.Headers.ContainsKey(HeaderNames.TraceParent))
            traceIdRelation = $"{context.Request.Headers[HeaderNames.TraceParent]},{context.TraceIdentifier}";

        using (LogContext.PushProperty("RequestTrace", traceIdRelation))
            await next.Invoke(context);
    }
}