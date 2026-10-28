using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalBraineAndSpine.Services.ViewModels.AccountViewModels
{
    public class OtpViewModel
    {       

        [Required]        
        public int Otp { get; set; }                      
        
    }
}
