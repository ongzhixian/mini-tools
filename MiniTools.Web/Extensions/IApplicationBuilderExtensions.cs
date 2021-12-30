using MiniTools.Web.Middleware;

namespace MiniTools.Web.Extensions;

public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRequestHeaderLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestHeaderLogging>();
    }
}
