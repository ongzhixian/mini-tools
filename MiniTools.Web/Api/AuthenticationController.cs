
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.Services;

namespace MiniTools.Web.Api;


[ApiController]
[Route("api/[controller]")]

public class UserIdentityController : ControllerBase
{
    private readonly ILogger<UserIdentityController> _logger;
    private readonly IConfiguration _configuration;
    private readonly AuthenticationService authenticationService;

    public UserIdentityController(ILogger<UserIdentityController> logger, IConfiguration configuration, 
        AuthenticationService authenticationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    [HttpPost]
    //[Route("api/authenticate")]
    public async Task<IActionResult> AuthenticateAsync([FromBody] LoginRequest model)
    {
        if (model == null || model.Username == null || model.Password == null)
            return Unauthorized();

        IOutcome oc = await authenticationService.GetValidUserAsync(model);

        if (!oc.Positive)
            return Unauthorized();

        //if (!await authenticationService.ValidCredentialsAsync(model))
        //    return Unauthorized();

        
        
        
    
        // var userRoles = await userManager.GetRolesAsync(user);

        // var authClaims = new List<Claim>
        //     {
        //         new Claim(ClaimTypes.Name, user.UserName),
        //         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //     };

        // foreach (var userRole in userRoles)
        // {
        //     authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        // }
        var authClaims = new List<Claim>();

        authClaims.Add(new Claim(ClaimTypes.Name, model.Username));

        string jwtSecret = _configuration["JWT:SecretKey"];
        string jwtValidIssuer = _configuration["JWT:ValidIssuer"];
        string jwtValidAudience = _configuration["JWT:ValidAudience"];

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

        var token = new JwtSecurityToken(
            issuer: jwtValidIssuer,
            audience: jwtValidAudience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }

    private bool IsValidCredentials()
    {
        return true;
    }
}