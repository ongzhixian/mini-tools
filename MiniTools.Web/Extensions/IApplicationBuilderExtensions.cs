using MiniTools.Web.Middleware;
using System.Diagnostics.CodeAnalysis;

namespace MiniTools.Web.Extensions;

[ExcludeFromCodeCoverage]
public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRequestHeaderLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestHeaderLogging>();
    }
}
