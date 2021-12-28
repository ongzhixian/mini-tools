using MiniTools.Web.Models;

namespace MiniTools.Web.Api.Requests;
public class LoginRequest : LoginViewModel
{
    public LoginRequest()
    {
    }

    public LoginRequest(LoginViewModel model)
    {
        this.Username = model.Username;
        this.Password = model.Password;
    }
}
