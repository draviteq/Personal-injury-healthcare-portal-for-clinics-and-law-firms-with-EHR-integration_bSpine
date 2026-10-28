using CapitalBraineAndSpine.Data.Entities.SettingEntities;
using System.ComponentModel.DataAnnotations;


namespace CapitalBraineAndSpine.Services.ViewModels.SettingViewModels
{
    public class RegionalSettingsViewModel : RegionalSettings
    {
        [Required]
        public new string TimeZone { get; set; }
    }
}
