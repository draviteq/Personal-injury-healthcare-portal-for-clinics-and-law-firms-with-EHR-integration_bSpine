using CapitalBraineAndSpine.Services.Infrastructures.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalBraineAndSpine.Services.ViewModels.SettingViewModels
{
    public class GeneralSettingsViewModel
    {
        public int Id {  get; set; }    
        public int SettingType { get; set; }


        [Required(ErrorMessage = "SettingValue is required.")]
        [StringLength(3, MinimumLength =1, ErrorMessage = "Setting value must be between 1 and 3 characters.")]
        public string SettingValue { get; set; }
    }
}
