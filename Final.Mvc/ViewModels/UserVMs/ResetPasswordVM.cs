﻿using System.ComponentModel.DataAnnotations;

namespace Final.Mvc.ViewModels.UserVMs
{
    public class ResetPasswordVM
    {
        public string Email { get; set; }
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
