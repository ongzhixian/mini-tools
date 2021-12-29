using Microsoft.AspNetCore.Mvc;

namespace MiniTools.Web.Extensions;

public static class LoggerExtensions
{
    private static class On
    {
        internal static readonly EventId MVC_VIEW = new EventId(1, "MVC View");
    }

    public static void LogMvcView(this ILogger logger, ControllerContext context, object data)
    {
        logger.LogInformation(On.MVC_VIEW, "{@controllerName}-{@actionName} {@data}",
            context.ActionDescriptor.ControllerName,
            context.ActionDescriptor.ActionName, data);
    }

    public static void LogMvcView(this ILogger logger, EventId eventId, ControllerContext context)
    {
        logger.LogInformation(eventId, "{@controllerName}-{@actionName}",
            context.ActionDescriptor.ControllerName,
            context.ActionDescriptor.ActionName);
    }
}
