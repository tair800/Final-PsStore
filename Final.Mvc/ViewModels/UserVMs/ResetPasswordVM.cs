using System.ComponentModel.DataAnnotations;

public class ResetPasswordVM
{
    public string Email { get; set; }

    public string Token { get; set; } // This token will be passed in the reset password URL

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
