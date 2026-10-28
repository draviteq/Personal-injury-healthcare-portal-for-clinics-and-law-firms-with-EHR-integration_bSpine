using CapitalBraineAndSpine.Data.Contexts;
using CapitalBraineAndSpine.Data.Entities.SettingEntities;
using CapitalBraineAndSpine.Service.Infrastructures.Helpers;
using CapitalBraineAndSpine.Services.Infrastructures.Enums;
using CapitalBraineAndSpine.Services.Services.Abstracts.SettingsAbstracts;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;
using Microsoft.EntityFrameworkCore;

namespace CapitalBraineAndSpine.Services.Services.SettingsServices
{
    public class SettingsService : ISettingsService
    {
        private readonly BaseDbContext _baseDbContext;
        public SettingsService(BaseDbContext baseDbContext)
        {
            _baseDbContext = baseDbContext;
        }

        /// <summary>
        /// Manage General Settings
        /// </summary>
        /// <param name="generalSettingsViewModel"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> ManageGeneralSettingsValue(GeneralSettingsViewModel generalSettingsViewModel, int Id)
        {
            if (generalSettingsViewModel != null)
            {
                GeneralSetting generalSetting;
                if (Id == 0)
                {
                    generalSetting = new() { CreatedDate = DateTime.Now };
                    _baseDbContext.Add(generalSetting);
                }
                else
                    generalSetting = await _baseDbContext.GeneralSettings.FindAsync(Id);


                generalSetting.SettingValue = generalSettingsViewModel.SettingValue;
                generalSetting.SettingType = Convert.ToInt32(GeneralSettingType.DownloadDataCharges);
                generalSetting.LastUpdated = CBSHelpers.CurrentLocalDateTime();


                await _baseDbContext.SaveChangesAsync();
                string responseMessage = Id == 0 ? "Created Successfully" : "Updated Successfully";
                return new ResponseViewModel(true, responseMessage);

            }
            return null;
        }

        /// <summary>
        /// Get details from general setting according setting type
        /// </summary>
        /// <param name="settingType"></param>
        /// <returns></returns>
        public async Task<GeneralSettingsViewModel> GetAllGeneralSettingsValues(int settingType)
        {
            if (settingType != 0)
            {
                var details = await _baseDbContext.GeneralSettings.Where(x => x.SettingType == settingType).Select(x => new GeneralSettingsViewModel
                {
                    Id = x.Id,
                    SettingType = x.SettingType,
                    SettingValue = x.SettingValue,

                }).FirstOrDefaultAsync();
                return details;
            }
            else
            {
                return null;
            }

        }
    }
}
