using CapitalBraineAndSpine.Services.ViewModels.AccountViewModels;
using CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels;
using CapitalBraineAndSpine.Services.ViewModels.LawFirmViewModels;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.LawFirmAbstracts
{
    public interface ILawFirmService
    {
        Task<RegisterViewModel> ManageLawFirmAsync(RegisterViewModel model);
        Task<List<RolePermissionVM>> GetRolePermissions(string targetRole);
        Task UpdatePermissionsAsync(RolePermissionViewModel rolePermissionVMs);
        Task<List<LawFirmVM>> GetLawFirmDetails(DataTableParamsVM dataTableParamsVM);
        Task<List<LawFirmStaffVM>> GetLawFirmStaffDetails(DataTableParamsVM dataTableParamsVM);
    }
}
