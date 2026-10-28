using HealthCare.Services.Infrastructures.Helpers;
using HealthCare.Services.Services.Abstracts.CommonAbstracts;
using HealthCare.Services.ViewModels.SettingViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HealthCare.Web.ViewComponents
{
    public class MenuAttorneyVC : ViewComponent
    {
        private readonly ICommonService _commonService;

        public MenuAttorneyVC(ICommonService commonService)
        {
            _commonService = commonService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            PermissionsVM products = await _commonService.GetMenuConfigByUserRole(Roles.Attorney);
            return View(products);
        }
    }
}
