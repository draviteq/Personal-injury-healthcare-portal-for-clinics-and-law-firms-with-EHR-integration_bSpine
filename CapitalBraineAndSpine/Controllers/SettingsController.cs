
using HealthCare.Data.Contexts;
using HealthCare.Data.Entities.IdentityEntities;
using HealthCare.Services.Infrastructures.Enums;
using HealthCare.Services.Infrastructures.Helpers;
using HealthCare.Services.Services.Abstracts.SettingsAbstracts;
using HealthCare.Services.ViewModels.SettingViewModels;
using HealthCare.Web.Infrastructures.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HealthCare.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SettingsController : CBSBaseController
    {
        private readonly ISettingsService _settingsService;
        private readonly BaseDbContext _baseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<CBSUser> _userManager;
        public SettingsController(UserManager<CBSUser> userManager, ISettingsService settingsService, IHttpContextAccessor httpContextAccessor, BaseDbContext baseDbContext) : base(userManager)
        {
            _userManager = userManager;
            _settingsService = settingsService;
            _baseDbContext = baseDbContext;
            _httpContextAccessor = httpContextAccessor;
        }



        /// <summary>
        /// General Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet("/[Controller]")]
        [Authorize(Roles = "SuperAdmin,LawFirm")]
        public async Task<IActionResult> GeneralSettings()
        {
            var user = _httpContextAccessor?.HttpContext?.User;

            var userV = await GetCurrentUserAsync();


            // Check if the user has any of the required roles
            if (RoleHelper.UserHasAnyRole(user, "superadmin", "lawfirm"))
            {
                return View();
            }

            return View();
        }

        /// <summary>
        /// General Setting
        /// </summary>
        /// <param name="generalSettingsViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GeneralSettings(GeneralSettingsViewModel generalSettingsViewModel)
        {

            if (ModelState.IsValid)
            {
                var existingModel = await _settingsService.GetAllGeneralSettingsValues(generalSettingsViewModel.SettingType);
                int Id = 0;

                if (existingModel != null)
                    Id = existingModel.Id;

                var statusDetails = await _settingsService.ManageGeneralSettingsValue(generalSettingsViewModel, Id);
                return Ok(GetAjaxResponse(statusDetails.Status, statusDetails.StatusMessage, statusDetails));

            }
            else
            {
                return Ok(GetAjaxResponse(false, "Validation error"));

            }

        }

        /// <summary>
        /// Get General Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGeneralSettings()
        {
            var model = await _settingsService.GetAllGeneralSettingsValues(Convert.ToInt32(GeneralSettingType.DownloadDataCharges));
            if (model != null)
                return Ok(GetAjaxResponse(true, "", model));
            else
                return RedirectToAction("GeneralSettings");

        }
    }
}
