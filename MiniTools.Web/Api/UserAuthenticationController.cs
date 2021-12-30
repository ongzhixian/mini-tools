using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MiniTools.Web.Api.Requests;
using MiniTools.Web.Api.Responses;
using MiniTools.Web.DataEntities;
using MiniTools.Web.Models;
using MiniTools.Web.Services;

namespace MiniTools.Web.Api;


[ApiController]
[Route("api/[controller]")]

public class UserAuthenticationController : ControllerBase
{
    private readonly ILogger<UserAuthenticationController> _logger;
    private readonly IConfiguration _configuration;
    private readonly AuthenticationService authenticationService;
    private readonly JwtService jwtService;
    
    public UserAuthenticationController(ILogger<UserAuthenticationController> logger, IConfiguration configuration
        , AuthenticationService authenticationService
        , JwtService jwtService
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        this.jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    }

    [HttpPost]
    //[Route("api/authenticate")]
    public async Task<IActionResult> AuthenticateAsync([FromBody] LoginRequest model)
    {
        if (model == null || model.Username == null || model.Password == null)
            return Unauthorized();

        OperationResult<UserAccount> op = await authenticationService.GetValidUserAsync(model);

        _logger.LogInformation(new EventId(234, "AuthAsync"), "{@res}", 12345 // System.Diagnostics.Activity.Current
            );


       
        if (!op.Success)
            return Unauthorized();

        if (op.Payload == null)
            return Unauthorized(); //TODO: Fix this misnomer

        // TODO: Create RoleService to get roles
        string[] userRoles = GetRoles(op.Payload.Username);

        // Setup claims

        var authClaims = new List<Claim>();
        
        authClaims.Add(new Claim(ClaimTypes.Name, op.Payload.Username));
        //authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        foreach (var userRole in userRoles)
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));

        JwtSecurityToken token = jwtService.CreateToken(authClaims);

        var res = new LoginResponse
        {
            Jwt = jwtService.ToCompactSerializationFormat(token),
            ExpiryDateTime = token.ValidTo
        };

        _logger.LogInformation(new EventId(234, "AuthAsync"), "{@res}", res);

        return Ok(res);
    }

    public string[] GetRoles(string username)
    {
        if (username == "dev")
            return new string[] {
                "Administrator",
                "Developer",
                "MyProfile"
            };

        return new string[] {
                "MyProfile"
            };
    }
}
