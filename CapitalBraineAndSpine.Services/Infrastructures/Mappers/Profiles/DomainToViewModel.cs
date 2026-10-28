
using AutoMapper;
using CapitalBraineAndSpine.Data.Entities.IdentityEntities;
using CapitalBraineAndSpine.Data.Entities.SettingEntities;
using CapitalBraineAndSpine.Services.ViewModels.AccountViewModels;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;

namespace CapitalBraineAndSpine.Service.Infrastructures.Mappers.Profiles
{
    public class DomainToViewModel : Profile
    {
        public DomainToViewModel()
        {
            CreateMap<CBSRole, RolesViewModel>();
            CreateMap<CBSUser, UserViewModel>();
            CreateMap<EmailTemplates, EmailTemplateViewModel>();
            CreateMap<RegionalSettings, RegionalSettingsViewModel>();
        }
    }
}
