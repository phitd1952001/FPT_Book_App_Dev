﻿using System.ComponentModel.DataAnnotations;

namespace FPT_Book_Khôi_Phi.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}