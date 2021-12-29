using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;
using MiniTools.Web.Services;

namespace MiniTools.Web.Controllers;

[AllowAnonymous]
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

    private readonly AuthenticationApiService authenticationApiService;

    public LoginController(ILogger<LoginController> logger, AuthenticationApiService authenticationApiService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this.authenticationApiService = authenticationApiService ?? throw new ArgumentNullException(nameof(authenticationApiService));

        logger.LogInformation(On.NEW, "{onEvent} - Controller [{controllerName}]", On.NEW, nameof(LoginController));
    }

    [AllowAnonymous]
    // [Route("/login")]
    public IActionResult Index()
    {
        // ControllerContext.ActionDescriptor.ControllerName
        
        logView();

        return View(new LoginViewModel{
            Username = "dev"
        });
    }

    [AllowAnonymous]
    [HttpPost]
    // [Route("/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IndexAsync(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(model);
        
        try
        {
            if (await authenticationApiService.IsValidCredentialsAsync(model))
            {
                // TODO: Get roles
                

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    //new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction(nameof(Index));
            }

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

    bool isValidCredentials()
    {
        return true;
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
