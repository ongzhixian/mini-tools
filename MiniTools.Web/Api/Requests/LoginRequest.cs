using MiniTools.Web.Models;

namespace MiniTools.Web.Api.Requests;

public class AddUserRequest : AddUserViewModel
{
    public AddUserRequest() { }

    public AddUserRequest(AddUserViewModel model)
    {
        this.Username = model.Username;
        this.Password = model.Password;
        this.LastName = model.LastName;
        this.FirstName = model.FirstName;
        this.Email = model.Email;
    }
}