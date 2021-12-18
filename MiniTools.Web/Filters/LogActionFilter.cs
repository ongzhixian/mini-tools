using Microsoft.AspNetCore.Mvc.Filters;

namespace MiniTools.Web.Filters;

public class LogActionFilterService : IActionFilter
{
    private readonly ILogger<LogActionFilterService> logger;

    public LogActionFilterService(ILogger<LogActionFilterService> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        logger.LogInformation("YYY1 LogActionFilterService -- OnActionExecuting");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        logger.LogInformation("YYY2 LogActionFilterService -- OnActionExecuted");
    }

    //     public void OnActionExecuting(ActionExecutingContext context)
    //     {
    //         // Do something before the action executes.
    //     }

    //     public void OnActionExecuted(ActionExecutedContext context)
    //     {
    //         // Do something after the action executes.
    //     }
}