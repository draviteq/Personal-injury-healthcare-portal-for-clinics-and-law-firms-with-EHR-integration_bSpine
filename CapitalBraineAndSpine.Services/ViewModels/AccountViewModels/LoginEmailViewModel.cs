using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalBraineAndSpine.Services.ViewModels.AccountViewModels
{

    public class LoginEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Id")]
        public string Email { get; set; }

        public string UserId { get; set; }
    }

}
