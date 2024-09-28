using System.ComponentModel.DataAnnotations;

namespace Final.Mvc.ViewModels.UserVMs
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        //public string Token { get; set; }
    }
}
