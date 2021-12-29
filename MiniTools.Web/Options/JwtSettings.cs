namespace MiniTools.Web.Options;

//string jwtSecret = _configuration["JWT:SecretKey"];
//string jwtValidIssuer = _configuration["JWT:ValidIssuer"];
//string jwtValidAudience = _configuration["JWT:ValidAudience"];

public class JwtSettings
{
    public string SecretKey { get; set; }
    
    public string ValidIssuer { get; set; }
    
    public string ValidAudience { get; set; }
}
