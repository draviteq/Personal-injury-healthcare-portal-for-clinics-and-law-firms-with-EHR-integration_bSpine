using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CapitalBraineAndSpine.Services.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Otp { get; set; }

        public bool IsSuccessLogin { get; set; }

        public string UserId { get; set; }
        public bool ResendOtp { get; set; }
        public string ErrorMessage { get; set; }
        public string ReturnUrl { get; set; }
    }
}
