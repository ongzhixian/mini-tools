using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTools.Web.Models;
using MiniTools.Web.Services;
using MiniTools.Web.Extensions;
using MiniTools.Web.Api.Responses;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

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

        try
        {
            OperationResult<LoginResponse> result = await authenticationApiService.IsValidCredentialsAsync(model);

            if (result.Success)
            {

                
                //JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                //if (!handler.CanReadToken(result.Payload.Jwt))
                //{
                //    // TODO: 
                //}


                IEnumerable<Claim>? claims = jwtService.GetClaims(result.Payload.Jwt);

                //string jwtSecret = _configuration["JWT:SecretKey"];
                //string jwtValidIssuer = _configuration["JWT:ValidIssuer"];
                //string jwtValidAudience = _configuration["JWT:ValidAudience"];

                //handler.ValidateToken(result.Payload.Jwt, new TokenValidationParameters
                //{
                //    ValidateIssuerSigningKey = true,
                //    ValidateIssuer = true,
                //    ValidateAudience = true,
                //    ValidIssuer = myIssuer,
                //    ValidAudience = myAudience,
                //    IssuerSigningKey = mySecurityKey
                //}, out SecurityToken validatedToken);


                //var claims = new List<Claim>
                //{
                //    new Claim(ClaimTypes.Name, model.Username),
                //    //new Claim("FullName", user.FullName),
                //    new Claim(ClaimTypes.Role, "Administrator"),
                //};

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

            return View(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, string.Empty);
            throw;
        }
    }


}
