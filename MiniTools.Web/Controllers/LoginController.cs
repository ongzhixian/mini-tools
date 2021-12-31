using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;
using MiniTools.Web.Services;
using MiniTools.Web.Extensions;
using MiniTools.Web.Api.Responses;

namespace MiniTools.Web.Controllers;

[AllowAnonymous]
public class LoginController : Controller
{
    private readonly ILogger<LoginController> logger;

    private readonly AuthenticationApiService authenticationApiService;

    private readonly JwtService jwtService;

    public LoginController(ILogger<LoginController> logger, AuthenticationApiService authenticationApiService, JwtService jwtService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        this.authenticationApiService = authenticationApiService ?? throw new ArgumentNullException(nameof(authenticationApiService));

        this.jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    }

    [AllowAnonymous]
    // [Route("/login")]
    public IActionResult Index()
    {
        var newModel = new LoginViewModel
        {
            Username = "dev"
        };

        logger.LogMvcView(ControllerContext, newModel);

        return View(newModel);
    }

    [AllowAnonymous]
    [HttpPost]
    // [Route("/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IndexAsync(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(model);

        logger.LogInformation(new EventId(123, "IndexAsync"), "{asd}", 1111);

        try
        {
            OperationResult<LoginResponse> result = await authenticationApiService.IsValidCredentialsAsync(model);

            if (!result.Success)
            {
                ViewBag.Alert = $"Invalid user credentials provided. {HttpContext.TraceIdentifier}";
                logger.LogMvcView(ControllerContext, model);
                return View(model);
            }

            if ((result.Payload == null) || string.IsNullOrWhiteSpace(result.Payload.Jwt))
            {
                ViewBag.Alert = "Invalid user credentials provided.";
                logger.LogMvcView(ControllerContext, model);
                return View(model);
            }

            IEnumerable<Claim> claims = jwtService.GetClaims(result.Payload.Jwt);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

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

            //return View(model);
        }
        catch (Exception ex)
        {
            ViewBag.Alert = $"Application error. Please contact system administrator with {HttpContext.TraceIdentifier}.";
            logger.LogError(new EventId(3923, "Error"), ex, "Error");
            logger.LogMvcView(ControllerContext, model);
            return View(model);
        }
    }
}
