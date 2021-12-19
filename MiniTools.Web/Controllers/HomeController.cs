using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;
using MiniTools.Web.Services;

namespace MiniTools.Web.Controllers;

public class HomeController : Controller
{
    private class On
    {
        internal static EventId NEW = new EventId(1, "New");
        internal static EventId VIEW_HOME = new EventId(101, "View home");
        internal static EventId VIEW_PRIVACY = new EventId(102, "View privacy");
    }
    private readonly ILogger<HomeController> logger;

    public HomeController(ILogger<HomeController> logger)
    {
        this.logger = logger ?? throw new Exception(nameof(logger));

        logger.LogInformation(On.NEW, "{onEvent} - Controller [{controllerName}]", On.NEW, nameof(HomeController));
    }

    public IActionResult Index()
    {
        logger.LogInformation(On.VIEW_HOME, "{onEvent} - Controller [{controllerName}]", On.VIEW_HOME, nameof(HomeController));

        return View();
    }

    public IActionResult Privacy()
    {
        logger.LogInformation(On.VIEW_PRIVACY, "{onEvent} - Controller [{controllerName}]", On.VIEW_PRIVACY, nameof(HomeController));

        return View();
    }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
