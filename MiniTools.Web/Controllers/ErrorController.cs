using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;

namespace MiniTools.Web.Controllers;

[AllowAnonymous]
public class ErrorController : Controller
{
    private class On
    {
        // public 
        // protected internal
        // protected
        // internal
        // private protected
        // private

        internal static EventId NEW = new EventId(1, "New");
        internal static EventId VIEW_ERROR = new EventId(2, "View error");
    }

    private readonly ILogger<ErrorController> logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        logger.LogInformation(On.NEW, "{onEvent} - Controller [{controllerName}]", On.NEW, nameof(ErrorController));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        string? requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        logger.LogInformation(On.VIEW_ERROR, "{onEvent} - RequestId [{requestId}]", On.VIEW_ERROR, requestId);

        return View(new ErrorViewModel { RequestId = requestId });
    }


    // [Route("/http-status/{code:int}")]
    // public IActionResult StatusCode(int code)
    // {
    //     // IExceptionHandlerFeature
    //     // This feature contains information about error from the original request
    //     // You examine the information in this feature to get original Exception, endpoint, path, routeValues
    //     var lastRoute = HttpContext.Features.Get<IExceptionHandlerFeature>();

    //     if (lastRoute != null)
    //     {
    //         if (lastRoute.Path != null)
    //             _logger.LogInformation("Path:        {0}", lastRoute?.Path);
    //         else
    //             _logger.LogInformation("Path:        N/A");

    //         // _logger.LogInformation("RouteValues: {0}", lastRoute?.RouteValues);
    //     }
    //     else 
    //     {
    //         _logger.LogInformation("LastRoute is null");
    //     }

    //     return View();
    // }


}