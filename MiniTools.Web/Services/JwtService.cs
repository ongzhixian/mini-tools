using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MiniTools.Web.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniTools.Web.Services;

public interface IJwtService
{
    JwtSecurityToken CreateToken(List<Claim> authClaims);

    string ToCompactSerializationFormat(JwtSecurityToken token);
    IEnumerable<Claim> GetClaims(string jwt);
}

public class JwtService : IJwtService
{
    //private readonly IConfiguration _configuration;

    private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;

    private readonly JwtSettings jwtSettings;

    public JwtService()
    {
        jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        jwtSettings = new JwtSettings();
    }

    public JwtService(IConfiguration configuration, IOptionsMonitor<JwtSettings> optionsMonitor) : this()
    {
        jwtSettings = optionsMonitor.Get("jwt");

        // DEBT: Do we need to check if values in jwtSettings are empty?

        jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    }

    public JwtSecurityToken CreateToken(List<Claim> authClaims)
    {
        //string jwtSecret = _configuration["JWT:SecretKey"];
        //string jwtValidIssuer = _configuration["JWT:ValidIssuer"];
        //string jwtValidAudience = _configuration["JWT:ValidAudience"];

        //string jwtSecret = jwtSettings.SecretKey;
        //string jwtValidIssuer = jwtSettings.ValidIssuer;
        //string jwtValidAudience = jwtSettings.ValidAudience;

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        var token = new JwtSecurityToken(
            issuer: jwtSettings.ValidIssuer,
            audience: jwtSettings.ValidAudience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }

    public string ToCompactSerializationFormat(JwtSecurityToken token)
    {
        return jwtSecurityTokenHandler.WriteToken(token);
    }


    public IEnumerable<Claim> GetClaims(string jwt)
    {
        if (!jwtSecurityTokenHandler.CanReadToken(jwt))
            throw new ArgumentException("Argument is not well-formed JWT", nameof(jwt));

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        ClaimsPrincipal? claimPrincipal = jwtSecurityTokenHandler.ValidateToken(
            jwt,
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings.ValidIssuer,
                ValidAudience = jwtSettings.ValidAudience,
                IssuerSigningKey = authSigningKey
            }, out _);

        return claimPrincipal.Claims;
    }

    //internal SecurityToken FromCompactSerializationFormat(string jwt)
    //{
    //    if (!jwtSecurityTokenHandler.CanReadToken(jwt))
    //        throw new ArgumentException("Argument is not well-formed JWT", nameof(jwt));

    //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

    //    SecurityToken token;

    //    ClaimsPrincipal? claimPrincipal = jwtSecurityTokenHandler.ValidateToken(
    //        jwt,
    //        new TokenValidationParameters
    //        {
    //            ValidateIssuerSigningKey = true,
    //            ValidateIssuer = true,
    //            ValidateAudience = true,
    //            ValidIssuer = jwtSettings.ValidIssuer,
    //            ValidAudience = jwtSettings.ValidIssuer,
    //            IssuerSigningKey = authSigningKey
    //        }, out token);

    //    return token;
    //}
}
