namespace MiniTools.Web.Api.Responses;

public class LoginResponse
{
    public string Jwt { get; set; }
    public DateTime ExpiryDateTime { get; set; }
}