namespace MiniTools.Web.Api.Requests;

public class AddUserRoleRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public AddUserRoleRequest() 
    { 
    }
}
