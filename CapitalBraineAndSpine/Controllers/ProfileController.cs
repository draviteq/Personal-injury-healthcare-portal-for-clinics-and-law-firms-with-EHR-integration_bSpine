using HealthCare.Data.Entities.IdentityEntities;
using HealthCare.Services.Services.Abstracts.CommonAbstracts;
using HealthCare.Services.Services.Abstracts.UserAbstracts;
using HealthCare.Services.ViewModels.ProfileViewModel;
using HealthCare.Web.Infrastructures.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HealthCare.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ProfileController : CBSBaseController
    {
        private readonly IUserService _userService;
        private readonly ICommonService _commonService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<CBSUser> _userManager;
        private readonly SignInManager<CBSUser> _signInManager;


        public ProfileController(IUserService userService, ICommonService commonService, IWebHostEnvironment webHostEnvironment,
            UserManager<CBSUser> userManager,
            SignInManager<CBSUser> signInManager) : base(userManager)
        {
            _userService = userService;
            _commonService = commonService;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;

        }



        /// <summary>
        /// Change Password
        /// </summary>
        /// <returns></returns>
        [HttpGet("/[Controller]/changepassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        #region MyProfileGetAndUpdateDetails

        /// <summary>
        ///  View Page Of My Profile
        /// </summary>
        /// <returns></returns>
        [HttpGet("/[Controller]/myprofile")]
        public IActionResult MyProfile()
        {
            return View();
        }

        /// <summary>
        /// Get My Profile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("getmyprofile")]
        public async Task<IActionResult> GetMyProfile()
        {
            MyProfileViewModel model = new MyProfileViewModel();

            // Get logged-In user
            var userId = (await GetCurrentUserAsync()).Id;
            //Get details From Database
            model = await _commonService.GetMyProfileDetailsAsync(userId);
            return Json(new { success = true, model, message = "" });
        }

        /// <summary>
        /// Update My Profile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        [HttpPost("updatemyprofile")]
        public async Task<IActionResult> UpdateMyProfile(IFormFile? PhotoFile, MyProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                // upload profile image
                string directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "profile_document/super_admin");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                var profileImageName = string.Empty;
                if (PhotoFile != null)
                {
                    FileInfo fileinfo = new FileInfo(PhotoFile.FileName);
                    profileImageName = DateTime.Now.ToFileTime() + fileinfo.Extension;
                    string filePath = Path.Combine(directoryPath, profileImageName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        PhotoFile.CopyTo(fileStream);
                    }
                    model.ProfilePhoto = profileImageName;
                }
                // Update userdetail in Database
                await _commonService.UpdateMyProfileAsync(model, profileImageName);
                return Ok(GetAjaxResponse(true, "Profile updated successfully"));

            }

            else
            {
                return Ok(GetAjaxResponse(false, "Validation error"));
            }
        }
        #endregion MyProfileGetAndUpdateDetails


        #region ChangePassword


        /// <summary>
        /// CHange Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check user
                var user = await _userManager.GetUserAsync(User);

                // redirect to login page if not a valid users
                if (user == null)
                {
                    return RedirectToAction("login", "account");
                }

                // Check either password is valid or not
                var isPaswdCorrect = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!isPaswdCorrect)
                {
                    return Json(new { success = false, result = "You have enter invalid current password." });
                }
                else
                {
                    if (isPaswdCorrect && model.CurrentPassword == model.NewPassword)
                    {
                        return Json(new { sucess = false, result = "Old and new password cannot be same." });
                    }
                    else
                    {
                        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }

                            return Json(new { success = false, validationerror = true, result.Errors });
                        }

                        await _signInManager.RefreshSignInAsync(user);
                        return Json(new { success = true, result = "Password updated successfully." });

                    }
                }

            }

            return View(model);
        }
    }
    #endregion ChangePassword
}
