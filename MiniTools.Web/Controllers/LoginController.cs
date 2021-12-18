using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;

namespace MiniTools.Web.Controllers;

public class LoginController : Controller
{
    private class On
    {
        internal static EventId NEW = new EventId(1, "New");

        internal static EventId VIEW = new EventId(100, "View");
        internal static EventId VIEW_LOGIN = new EventId(101, "View login");
        internal static EventId VIEW_LOGIN_POST = new EventId(102, "View login (POST)");
    }
    private readonly ILogger<LoginController> logger;

    public LoginController(ILogger<LoginController> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        logger.LogInformation(On.NEW, "{onEvent} - Controller [{controllerName}]", On.NEW, nameof(LoginController));
    }

    [AllowAnonymous]
    // [Route("/login")]
    public IActionResult Index()
    {
        // ControllerContext.ActionDescriptor.ControllerName
        
        logView();

        return View(new LoginViewModel{
            Username = "test@test.local"
        });
    }

    [AllowAnonymous]
    [HttpPost]
    // [Route("/login")]
    [ValidateAntiForgeryToken]
    public IActionResult Index(LoginViewModel model)
    {
        try
        {
            logger.LogInformation(model.Username);
            logger.LogInformation(model.Password);

            // this.Request.Method
            // ControllerContext.ActionDescriptor.ControllerName

            logView();
            

            return View(model);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private void logView()
    {
        logger.LogInformation(On.VIEW, "{onEvent} - Controller [{controller}], Action [{action}]", 
        On.VIEW, 
        ControllerContext.ActionDescriptor.ControllerName, 
        ControllerContext.ActionDescriptor.ActionName);
    }

    // public IActionResult Index()
    // {
    //     return View(new LoginViewModel());
    // }

    // [AllowAnonymous]
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public IActionResult Login(LoginViewModel model)
    // {
    //     // string clsMtdName = MetaInfoHelper.ClsMtdName;

    //     // log.LogInformation(LogEvent.Start, "{clsMtdName} ({state})", clsMtdName, LogEvent.Start);

    //     try
    //     {

    //         // Console.WriteLine(model.Username);
    //         // Console.WriteLine(model.Password);
    //         // log.LogInformation(LogEvent.End, "{clsMtdName} ({state}) ", clsMtdName, LogEvent.End);

    //         _logger.LogInformation(model.Username);
    //         _logger.LogInformation(model.Password);

    //         //return RedirectToAction("DiceGame", "Game", model);

    //         // model.WagerResult.WagerType = model.WagerType;
    //         // model.WagerResult.Amount = model.Amount + (model.Amount * 1);

    //         return View(model);
    //     }
    //     catch (Exception ex)
    //     {
    //         // log.LogError(LogEvent.Error, ex, "{clsMtdName} ({state}) ", clsMtdName, LogEvent.Error);
    //         throw;
    //     }
    // }

    // public IActionResult Privacy()
    // {
    //     return View();
    // }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
