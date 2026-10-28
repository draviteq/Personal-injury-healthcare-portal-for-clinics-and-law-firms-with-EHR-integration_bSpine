using CapitalBraineAndSpine.Data.Entities.SettingEntities;
using CapitalBraineAndSpine.Services.ViewModels.ProfileViewModel;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.SettingsAbstracts
{
    public interface ISettingsService
    {
        Task<ResponseViewModel> ManageGeneralSettingsValue(GeneralSettingsViewModel generalSettingsViewModel, int Id);

        Task<GeneralSettingsViewModel> GetAllGeneralSettingsValues(int settingType);
       
    }
}
