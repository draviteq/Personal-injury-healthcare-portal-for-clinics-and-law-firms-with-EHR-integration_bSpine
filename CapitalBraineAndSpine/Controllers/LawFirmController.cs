using HealthCare.Data.Entities.IdentityEntities;
using HealthCare.Services.Infrastructures.Enums;
using HealthCare.Services.Infrastructures.Helpers;
using HealthCare.Services.Services.Abstracts.CommonAbstracts;
using HealthCare.Services.Services.Abstracts.LawFirmAbstracts;
using HealthCare.Services.Services.Abstracts.UserAbstracts;
using HealthCare.Services.ViewModels.AccountViewModels;
using HealthCare.Services.ViewModels.DataDownloadRequestViewModels;
using HealthCare.Services.ViewModels.LawFirmViewModels;
using HealthCare.Web.Infrastructures.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCare.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [Authorize(Roles = "superadmin,staff,lawfirm")]
    public class LawFirmController : CBSBaseController
    {
        private readonly UserManager<CBSUser> _userManager;
        private readonly ICommonService _commonService;
        private readonly ILawFirmService _lawFirmService;
        private readonly IUserService _userService;

        public LawFirmController(UserManager<CBSUser> userManager, ICommonService commonService
            , ILawFirmService lawFirmService
            , IUserService userService
            ) : base(userManager)
        {
            _userManager = userManager;
            _commonService = commonService;
            _lawFirmService = lawFirmService;
            _userService = userService;
        }

        /// <summary>
        /// Law Firm List
        /// </summary>
        /// <returns></returns>
        [HttpGet("/[Controller]")]
        public async Task<IActionResult> Index()
        {
            RolePermissionViewModel rolePermissionViewModel = new RolePermissionViewModel();
            rolePermissionViewModel.TargetRole = Roles.LawFirm;
            rolePermissionViewModel.RolePermissions = await _lawFirmService.GetRolePermissions(rolePermissionViewModel.TargetRole);
            return View(rolePermissionViewModel);
        }

        /// <summary>
        /// Manage LawFirm
        /// </summary>
        /// <returns></returns>      
        [HttpGet("/[controller]/Manage")]
        public async Task<IActionResult> Manage()
        {
            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                StateList = await _userService.StateListAsync(),
            };

            return View(registerViewModel);
        }

        #region Api's

        /// <summary>
        /// Manage Law Firm Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpGet("details")]
        public async Task<IActionResult> Details(string userId)
        {
            var loggedInUser = await GetCurrentUserAsync();
            var (result, message) = await _commonService.GetUserDetailsAsync(userId, loggedInUser.Id);

            if (!string.IsNullOrEmpty(message))
                return Json(GetAjaxResponse(false, string.Empty, result));

            return Json(GetAjaxResponse(true, string.Empty, result));
        }

        /// <summary>
        /// Manage Law Firm
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("manage")]
        public async Task<IActionResult> ManageLawFirm(RegisterViewModel model)
        {
            var role = new CBSRole { Name = Roles.LawFirm, Caption = Roles.LawFirm, Prefix = RolesPrefix.LawFirmPrefix, DisplayName = RoleDisplayName.LawFirm };
            var (result, message) = await _commonService.CreateUpdateUserAsync(model, role, (await GetCurrentUserAsync()).Id, (int)SourceType.ActivityLog);
            return Json(new { success = result, message = message });
        }


        /// <summary>
        /// Update Permissions
        /// </summary>
        /// <param name="rolePermissionVMs"></param>
        /// <returns></returns>
        [HttpPost("updatePermissions")]

        public async Task<IActionResult> UpdatePermissions(RolePermissionViewModel rolePermissionVMs)
        {
            await _lawFirmService.UpdatePermissionsAsync(rolePermissionVMs);
            return Ok();
        }

        /// <summary>
        /// Get Law Firm details
        /// </summary>
        /// <returns></returns>
        [HttpPost("getLawFirmDetails")]

        public async Task<IActionResult> GetLawFirmDetails()
        {
            DataTableParamsVM dataTableParamsVM = new DataTableParamsVM
            {
                userId = (await GetCurrentUserAsync()).Id,//Need to get it from session dynamically;
                draw = Request.Form["draw"].FirstOrDefault(),
                start = Request.Form["start"].FirstOrDefault(),
                length = Request.Form["length"].FirstOrDefault(),
                sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault(),
                sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault(),
                searchValue = Request.Form["search[value]"].FirstOrDefault()?.ToLower(),
                pageSize = Request.Form["length"].FirstOrDefault() != null ? Convert.ToInt32(Request.Form["length"].FirstOrDefault()) : 0,
                skip = Request.Form["start"].FirstOrDefault() != null ? Convert.ToInt32(Request.Form["start"].FirstOrDefault()) : 0,
                recordsTotal = 0,
            };
            var _lawFirmList = await _lawFirmService.GetLawFirmDetails(dataTableParamsVM);
            var jsonData = new { draw = dataTableParamsVM.draw, recordsFiltered = dataTableParamsVM.recordsTotal, recordsTotal = dataTableParamsVM.recordsTotal, data = _lawFirmList };
            return Ok(jsonData);
        }
        #endregion

    }
}
