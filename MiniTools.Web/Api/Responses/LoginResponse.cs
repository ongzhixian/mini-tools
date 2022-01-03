namespace MiniTools.Web.Api.Responses;

public class LoginResponse
{
    public string Jwt { get; set; } = String.Empty;

    public DateTime ExpiryDateTime { get; set; }
}