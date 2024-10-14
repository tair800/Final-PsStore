using System.ComponentModel.DataAnnotations;

namespace Final.Application.Dtos.UserDtos
{
    public class AdminPasswordChangeDto
    {

        public string Id { get; set; }

        public string FullName { get; set; }

        public string Username { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public string Password { get; set; }

    }

}
