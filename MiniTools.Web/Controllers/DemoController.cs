using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;
using MiniTools.Web.Services;

namespace MiniTools.Web.Controllers;

[AllowAnonymous]
public class DemoController : Controller
{
    private class On
    {
        internal static EventId NEW = new EventId(1, "New");
        internal static EventId VIEW = new EventId(101, "View");
    }

    private readonly ILogger<HomeController> logger;
    // private readonly ILoginService loginService;

    public DemoController(ILogger<HomeController> logger)
    {
        this.logger = logger ?? throw new Exception(nameof(logger));
        // this.loginService = loginService ?? throw new Exception(nameof(loginService));

        // logger.LogInformation(On.NEW, "[{eventSrc}] - {onEvent}", nameof(HomeController), On.NEW);
        Log(On.NEW);
    }


    public IActionResult Index()
    {
        // logger.LogInformation(On.VIEW_HOME, "{onEvent} - Controller [{controllerName}]", On.VIEW_HOME, nameof(HomeController));

        Log(On.VIEW);

        return View();
    }

    [HttpPost]
    public IActionResult Test1()
    {
        // logger.LogInformation(On.VIEW_HOME, "{onEvent} - Controller [{controllerName}]", On.VIEW_HOME, nameof(HomeController));
        Log(On.VIEW);

        // 

        return View();
    }

    private void Log(EventId eventId)
    {
        string? requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        // var controllerName = ControllerContext.ActionDescriptor.ControllerName;
        // var action = ControllerContext.ActionDescriptor.ActionName;
        //string eventSrc=  $"{controllerName}:{action}";
        string eventSrc = nameof(DemoController);

        // When NEW, we only have ControllerContext
        // ControllerContext is null: False
        // ActionDescriptor is null: True

        // When VIEW, we have both ControllerContext and ActionDescriptor
        // ControllerContext is null: False
        // ActionDescriptor is null: False

        // logger.LogInformation("ControllerContext is null: {isNull}", (ControllerContext == null));
        // logger.LogInformation("ActionDescriptor is null: {isNull}", (ControllerContext.ActionDescriptor == null));
        // logger.LogInformation("ControllerName is null: {isNull}", (ControllerContext.ActionDescriptor.ControllerName == null));
        // logger.LogInformation("ActionName is null: {isNull}", (ControllerContext.ActionDescriptor.ActionName == null));

        logger.LogInformation(eventId, "ex1 [{eventSrc}] - {onEvent} [{requestId}]", eventSrc, eventId, requestId);
    }

}
