using System.ComponentModel.DataAnnotations;

namespace Final.Mvc.Areas.AdminArea.ViewModels.UserVMs
{
    public class UserUpdateVM
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Username { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
