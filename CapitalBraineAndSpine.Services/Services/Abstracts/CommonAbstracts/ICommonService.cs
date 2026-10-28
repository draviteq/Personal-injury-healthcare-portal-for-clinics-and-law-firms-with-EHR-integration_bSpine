using CapitalBraineAndSpine.Data.Entities.IdentityEntities;
using CapitalBraineAndSpine.Services.ViewModels.AccountViewModels;
using CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels;
using CapitalBraineAndSpine.Services.ViewModels.LawFirmViewModels;
using CapitalBraineAndSpine.Services.ViewModels.ProfileViewModel;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.CommonAbstracts
{
    public interface ICommonService
    {

        /// <summary>
        /// Create New User
        /// </summary>
        /// <param name="model"></param>
        /// <param name="role"></param>
        /// <param name="loggedInUserId"></param>
        /// <param name="lawFirmOrSuperAdminId"></param>
        /// <returns></returns>
        Task<(bool, string)> CreateUpdateUserAsync(RegisterViewModel model, CBSRole role, string loggedInUserId, int sourceType);

        /// <summary>
        /// Get my profile details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<MyProfileViewModel> GetMyProfileDetailsAsync(string userId);


        /// <summary>
        /// Update my profile Details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="profileImageName"></param>
        /// <returns></returns>
        Task<bool> UpdateMyProfileAsync(MyProfileViewModel model, string profileImageName);


        /// <summary>
        /// Get Law Firm Details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="role"></param>
        /// <param name="loggedInUserId"></param>
        /// <returns></returns>
        Task<(RegisterViewModel, string)> GetUserDetailsAsync(string userId, string loggedInUserId = "");

        /// <summary>
        /// Get Staff Details
        /// </summary>
        /// <param name="dataTableParamsVM"></param>
        /// <returns></returns>
        Task<List<LawFirmStaffVM>> GetStaffList(DataTableParamsVM dataTableParamsVM);
        RolesViewModel GetUserRoleByUserId(string id);

        Task<bool> ActiveDeactiveUserAsync(string id, bool isActive, string loggedInUserId);
        Task<PermissionsVM> GetMenuConfigByUserRole(string cBSAdmin);
    }
}
