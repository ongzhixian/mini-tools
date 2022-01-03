using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MiniTools.Web.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniTools.Web.Services
{
    public class JwtService
    {
        //private readonly IConfiguration _configuration;

        private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;

        private readonly JwtSettings jwtSettings;

        public JwtService(IConfiguration configuration, IOptionsMonitor<JwtSettings> optionsMonitor)
        {
            jwtSettings = optionsMonitor.Get("jwt");

            // DEBT: Do we need to check if values in jwtSettings are empty?
            
            jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        internal JwtSecurityToken CreateToken(List<Claim> authClaims)
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

        internal string ToCompactSerializationFormat(JwtSecurityToken token)
        {
            return jwtSecurityTokenHandler.WriteToken(token);
        }


        internal IEnumerable<Claim> GetClaims(string jwt)
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
                    ValidAudience = jwtSettings.ValidIssuer,
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
}
