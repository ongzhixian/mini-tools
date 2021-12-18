using Microsoft.AspNetCore.Mvc.Filters;

namespace MiniTools.Web.Filters;

public class LogResultFilterService : IResultFilter
{
    private readonly ILogger<LogResultFilterService> logger;

    public LogResultFilterService(ILogger<LogResultFilterService> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        logger.LogInformation("XXX2 LogResultFilterService -- OnResultExecuting");

        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];
        var unknown = context.RouteData.Values["unknown"];

        //logger.LogInformation(unknown);

        foreach (var item in context.RouteData.Values)
        {
            logger.LogInformation(item.Key);// action, controller
        }
        
    }
    public void OnResultExecuted(ResultExecutedContext context)
    {
        logger.LogInformation("XXX1 LogResultFilterService -- OnResultExecuted");
    }

    // public void OnResultExecuting(ResultExecutingContext context)
    // {
    //     _logger.LogInformation(
    //         $"- {nameof(LoggingResponseHeaderFilterService)}.{nameof(OnResultExecuting)}");
    //     context.HttpContext.Response.Headers.Add(
    //         nameof(OnResultExecuting), nameof(LoggingResponseHeaderFilterService));
    // }

    // public void OnResultExecuted(ResultExecutedContext context)
    // {
    //     _logger.LogInformation(
    //         $"- {nameof(LoggingResponseHeaderFilterService)}.{nameof(OnResultExecuted)}");
    // }
}