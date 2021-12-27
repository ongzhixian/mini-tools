using System.ComponentModel.DataAnnotations;

namespace MiniTools.Web.DataEntities
{
    public enum UserAccountStatus
    {
        Inactive,
        Active,
        Suspend
    }

    public class UserAccount
    {
        [Key]
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public UserAccountStatus Status { get; set; } = UserAccountStatus.Inactive;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime PasswordExpiryDate { get; set; } = DateTime.MaxValue;

        public UserAccount()
        {
        }

        public UserAccount(UserAccount copy)
        {
            this.Username = copy.Username;
            this.Password = copy.Password;
            this.Status = copy.Status;
            this.DateCreated = copy.DateCreated;
            this.PasswordExpiryDate = copy.PasswordExpiryDate;
        }
    }
}
