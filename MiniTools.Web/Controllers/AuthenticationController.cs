using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;

namespace MiniTools.Web.Controllers;

public class AuthenticationController : Controller
{
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(ILogger<AuthenticationController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult Index()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        // string clsMtdName = MetaInfoHelper.ClsMtdName;

        // log.LogInformation(LogEvent.Start, "{clsMtdName} ({state})", clsMtdName, LogEvent.Start);

        try
        {

            // Console.WriteLine(model.Username);
            // Console.WriteLine(model.Password);
            // log.LogInformation(LogEvent.End, "{clsMtdName} ({state}) ", clsMtdName, LogEvent.End);

            _logger.LogInformation(model.Username);
            _logger.LogInformation(model.Password);

            //return RedirectToAction("DiceGame", "Game", model);

            // model.WagerResult.WagerType = model.WagerType;
            // model.WagerResult.Amount = model.Amount + (model.Amount * 1);

            return View(model);
        }
        catch (Exception ex)
        {
            // log.LogError(LogEvent.Error, ex, "{clsMtdName} ({state}) ", clsMtdName, LogEvent.Error);
            throw;
        }
    }

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
