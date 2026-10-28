using AutoMapper;
using CapitalBraineAndSpine.Data.Entities.SettingEntities;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;

namespace CapitalBraineAndSpine.Service.Infrastructures.Mappers.Profiles
{
    public class ViewModelToDomain : Profile
    {
        public ViewModelToDomain()
        {
            CreateMap<RegionalSettingsViewModel, RegionalSettings>();
        }
    }
}
