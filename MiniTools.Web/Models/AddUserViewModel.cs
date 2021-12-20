using System.ComponentModel.DataAnnotations;

namespace MiniTools.Web.Models
{
    public class AddUserViewModel
    {
        [Display(Name = "Username", Description ="Username of user.")]
        public string Username { get; set; }

        [Display(Name = "Password", Description = "Password of user.")]
        public string Password { get; set; }

        [Display(Name = "First name", Description = "First name of user.")]
        public string FirstName { get; set; }

        [Display(Name = "Last name", Description = "Last name of user.")]
        public string LastName { get; set; }

        [Display(Name = "Email", Description = "Email address of user.")]
        public string Email { get; set; }


    }
}
