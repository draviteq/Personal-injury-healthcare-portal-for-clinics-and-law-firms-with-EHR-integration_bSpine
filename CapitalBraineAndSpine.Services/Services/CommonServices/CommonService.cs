using AutoMapper;
using CapitalBraineAndSpine.Data.Contexts;
using CapitalBraineAndSpine.Data.Entities.IdentityEntities;
using CapitalBraineAndSpine.Service.Infrastructures.Helpers;
using CapitalBraineAndSpine.Services.Infrastructures.Enums;
using CapitalBraineAndSpine.Services.Infrastructures.Helpers;
using CapitalBraineAndSpine.Services.Services.Abstracts.AuditLogAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.CommonAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.EmailAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.NotificationAbstracts;
using CapitalBraineAndSpine.Services.ViewModels.AccountViewModels;
using CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels;
using CapitalBraineAndSpine.Services.ViewModels.LawFirmViewModels;
using CapitalBraineAndSpine.Services.ViewModels.NotificationViewModels;
using CapitalBraineAndSpine.Services.ViewModels.ProfileViewModel;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections;
namespace CapitalBraineAndSpine.Services.Services.CommonServices
{
    public class CommonService : ICommonService
    {
        private readonly BaseDbContext _baseDbContext;
        private readonly IEmailService _emailService;
        private readonly UserManager<CBSUser> _userManager;
        private readonly RoleManager<CBSRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;
        private readonly IAuditLogService _activityLogService;

        public CommonService(BaseDbContext baseDbContext
            , IEmailService emailService
            , UserManager<CBSUser> userManager
            , RoleManager<CBSRole> roleManager
            , IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor
            , IMapper mapper
            , INotificationService notificationService
            , IAuditLogService activityLogService
            )
        {
            _baseDbContext = baseDbContext;
            _emailService = emailService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _activityLogService = activityLogService;
        }

        /// <summary>
        /// Get User Details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<(RegisterViewModel, string)> GetUserDetailsAsync(string userId, string loggedInUserId = "")
        {
            //if (string.IsNullOrEmpty(userId))
            //    userId = loggedInUserId;

            var userDetails = await _baseDbContext.Users.Where(x => x.Id == userId)
                .Join(_baseDbContext.UserRoles,
                u => u.Id,
                   ur => ur.UserId,
                   (u, ur) => new { U = u, UR = ur })
                   .Join(
                   _baseDbContext.Roles,
                   urr => urr.UR.RoleId,
                   r => r.Id,
                   (urr, r) => new { URR = urr, R = r })
                .Select(x => new RegisterViewModel
                {
                    Id = x.URR.U.Id,
                    FirstName = x.URR.U.FirstName,
                    LastName = x.URR.U.LastName,
                    LawFirmName = x.URR.U.LawFirmName,
                    PhoneNumber = x.URR.U.PhoneNumber,
                    Email = x.URR.U.Email,
                    Address = x.URR.U.Address,
                    City = x.URR.U.City,
                    State = x.URR.U.State,
                    Zip = x.URR.U.Zip,
                    CreatedDate = x.URR.U.CreatedDate,
                    RoleName = x.R.Name

                }).FirstOrDefaultAsync();

            if (userDetails == null)
                return (null, "Staff does not exists");

            return (userDetails, string.Empty);
        }

        /// <summary>
        /// Create New User
        /// </summary>
        /// <param name="model"></param>
        /// <param name="role"></param>
        /// <param name="loggedInUserId"></param>
        /// <returns></returns>
        public async Task<(bool, string)> CreateUpdateUserAsync(RegisterViewModel model, CBSRole role, string loggedInUserId, int sourceType)
        {
            var message = string.Empty;
            bool isAddressRequired = false;


            // Get logged In user role
            var loggedInUserRole = GetUserRoleByUserId(loggedInUserId);

            if (model.Id != null)
            {
                // Get user details
                var user = await _baseDbContext.Users.Where(x => x.Id == model.Id).FirstOrDefaultAsync();

                // Get user current role
                var userRoles = await _userManager.GetRolesAsync(user);
                var currentRole = await _roleManager.FindByNameAsync(userRoles.FirstOrDefault());


                if (role.Name != currentRole.ToString())
                {
                    // Remove current role
                    var removeRole = await _userManager.RemoveFromRoleAsync(user, userRoles.FirstOrDefault());

                    // Assign role to user
                    if (!await _userManager.IsInRoleAsync(user, role.Name))
                        await _userManager.AddToRoleAsync(user, role.Name);

                    // Address only required for LawFirm                    
                    if (role.Name == Roles.LawFirm)
                        isAddressRequired = true;
                }

                // Address only required for LawFirm                    
                if (role.Name == Roles.LawFirm)
                    isAddressRequired = true;

                // Check is user not found
                if (user == null)
                {
                    message = "No record found.";
                    return (false, message);
                }

                if (user != null)
                {
                    // Do not update email

                    //user.Email = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = isAddressRequired == true ? model.Address : string.Empty;
                    user.PhoneNumber = model.PhoneNumber;
                    user.ParentUserId = model.ParentUserId != null ? model.ParentUserId : loggedInUserId;
                    user.City = isAddressRequired == true ? model.City : string.Empty;
                    user.State = isAddressRequired == true ? model.State : string.Empty;
                    user.Zip = isAddressRequired == true ? model.Zip : string.Empty;
                    user.LawFirmName = model.LawFirmName;
                    user.UpdatedDate = CBSHelpers.CurrentLocalDateTime();

                    _baseDbContext.Update(user);

                    // Create Audit log

                    if (user != null)
                        await _activityLogService.AuditLogsAsync(false, user.Id, currentRole.Name);


                    await _baseDbContext.SaveChangesAsync();
                }

                // Create Notification
                NotificationViewModel notification = new NotificationViewModel()
                {
                    TriggerByUserId = loggedInUserId,
                    TriggerForUserId = loggedInUserRole.Name == Roles.SuperAdmin ? loggedInUserId : null, //currentRole.Name == Roles.LawFirm ? loggedInUserId : null,
                    SourceType = sourceType,

                    Type = currentRole.Name == Roles.LawFirm ? (int)NotificationType.UpdatingLawFirm :
                            currentRole.Name == Roles.Attorney ? (int)NotificationType.UpdatingLawFirmStaff :
                            //currentRole.Name == Roles.LFStaff ? (int)NotificationType.UpdatingLawFirm :
                            role.Name == Roles.CBSAdmin ? (int)NotificationType.CBSAdminStaffUpdate :
                            currentRole.Name == Roles.CBSStaff ? (int)NotificationType.Updatingstaff : 0,

                    PatientOrUserId = model.Id

                };

                _notificationService.CreateNotification(notification, loggedInUserId);
                return (true, "Record updated successfully.");
            }
            else
            {

                var roleName = string.Empty;

                // Get user on the basis of Email
                var checkEmail = await _baseDbContext.Users.Where(x => x.Email == model.Email).FirstOrDefaultAsync();

                // Check is user found
                if (checkEmail != null)
                {
                    message = "Email already exists.";
                    return (false, message);
                }





                // Address only required for LawFirm                    
                if (role.Name == Roles.LawFirm)
                    isAddressRequired = true;

                var user = new CBSUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = true,
                    Address = isAddressRequired == true ? model.Address : string.Empty,
                    City = isAddressRequired == true ? model.City : string.Empty,
                    State = isAddressRequired == true ? model.State : string.Empty,
                    Zip = isAddressRequired == true ? model.Zip : string.Empty,
                    PhoneNumber = model.PhoneNumber,
                    LawFirmName = model.LawFirmName,
                    IsActive = true, // By default all user will be active
                    CreatedDate = CBSHelpers.CurrentLocalDateTime(),
                    UpdatedDate = CBSHelpers.CurrentLocalDateTime(),
                    // This field used to check under which Law Firm/SuperAdmin Staff is working
                    ParentUserId = model.ParentUserId != null ? model.ParentUserId : loggedInUserId,
                    UserCreatedBy = model.ParentUserId != null ? model.ParentUserId : loggedInUserId
                };

                // Generate random password as It is mandatory
                model.Password = CBSHelpers.GeneratePassword(12);




                // Create user record in Database
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Create role if not exists
                    if (!await _roleManager.RoleExistsAsync(role.Name))
                        await _roleManager.CreateAsync(role);

                    // Assign role to user if not assign
                    if (!await _userManager.IsInRoleAsync(user, role.Name))
                        await _userManager.AddToRoleAsync(user, role.Name);

                    // Get user details
                    var userDetails = await _baseDbContext.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync();

                    if (userDetails != null)
                    {
                        // Get user role
                        var userRole = await _userManager.GetRolesAsync(user);

                        // Get Max Prefix Values of role from users
                        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

                        roleName = role.Name;

                        // Get max id
                        var maxPrefixId = usersInRole.OrderByDescending(x => x.PrefixVal).Max(x => x.PrefixVal);

                        // Create customUserId
                        var customUserId = role != null ? $"{role.Prefix}{(maxPrefixId > 0 ? maxPrefixId + 1 : 1)}" : string.Empty;

                        userDetails.CustomUserId = customUserId;
                        userDetails.PrefixVal = maxPrefixId > 0 ? maxPrefixId + 1 : 1;





                        // Update user details
                        _baseDbContext.Users.Update(userDetails);

                        // Create Audit log
                        await _activityLogService.AuditLogsAsync(true, userDetails.Id, roleName);

                        await _baseDbContext.SaveChangesAsync();

                        // Base url from AppSettings
                        var baseUrl = Convert.ToString(_configuration["WebsiteUrl:url"]);

                        // Set value in template
                        var tbl = new Hashtable
                        {
                            { "@FirstName", model.FirstName },
                            { "@LastName", model.LastName },
                            { "@Username", model.Email },
                            { "@baseurl", $"{baseUrl}account/loginwithemail" }
                        };

                        // Load Email Template
                        string bodyContent = _emailService.GetMailBody("NotifyUser.html", tbl);

                        EmailViewModel emailModel = new EmailViewModel { To = user.Email, Subject = "Your account created", Body = bodyContent };

                        // Create Notification
                        NotificationViewModel notification = new NotificationViewModel()
                        {
                            TriggerByUserId = loggedInUserId,
                            TriggerForUserId = loggedInUserRole.Name == Roles.SuperAdmin ? loggedInUserId : null,
                            SourceType = sourceType,
                            Type = role.Name == Roles.LawFirm ? (int)NotificationType.AddingLawFirm :
                                    role.Name == Roles.Attorney ? (int)NotificationType.AddingLawFirmStaff :
                                    //role.Name == Roles.LFStaff ? (int)NotificationType.UpdatingLawFirm :
                                    role.Name == Roles.CBSAdmin ? (int)NotificationType.CBSAdminStaffAdd :
                            role.Name == Roles.CBSStaff ? (int)NotificationType.Addingstaff : 0,
                            PatientOrUserId = user.Id

                        };

                        var status = _notificationService.CreateNotification(notification, loggedInUserId);



                        if (status)
                            await _emailService.SendEmailAsync(emailModel);

                    }
                }


                return (true, $"{roleName} created successfully.");
            }
        }


        /// <summary>
        /// Get user profile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<MyProfileViewModel> GetMyProfileDetailsAsync(string userId)
        {
            try
            {
                //dummy image 
                var dummyImage = "/profile_document/super_admin/PlaceholderPng.png";

                // Get user details
                var details = await _baseDbContext.Users.Where(x => x.Id == userId)
                    .Join(_baseDbContext.UserRoles,
                u => u.Id,
                   ur => ur.UserId,
                   (u, ur) => new { U = u, UR = ur })
                   .Join(
                   _baseDbContext.Roles,
                   urr => urr.UR.RoleId,
                   r => r.Id,
                   (urr, r) => new { URR = urr, R = r })
                    .Select(x => new MyProfileViewModel
                    {
                        Id = x.URR.U.Id,
                        FirstName = x.URR.U.FirstName,
                        LastName = x.URR.U.LastName,
                        PhoneNumber = x.URR.U.PhoneNumber,
                        ProfilePhotoPath = File.Exists($"/profile_document/super_admin/{x.URR.U.ProfileImage}") ? $"/profile_document/super_admin/{x.URR.U.ProfileImage}" : dummyImage,
                        Email = x.URR.U.Email,
                        Address = x.URR.U.Address,
                        RoleName = x.R.DisplayName

                    }).FirstOrDefaultAsync();

                // Check if ProfilePhotoPath is null or empty, and set to placeholder image if it is
                if (string.IsNullOrEmpty(details?.ProfilePhotoPath) || details.ProfilePhotoPath.EndsWith("/super_admin/"))
                {
                    details.ProfilePhotoPath = "~/assets/images/pngs/1.png";
                }

                return details;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        /// <summary>
        /// Update my profile details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMyProfileAsync(MyProfileViewModel model, string profileImageName)
        {
            try
            {
                // Get user details
                var user = await _baseDbContext.Users.Where(x => x.Id == model.Id).FirstOrDefaultAsync();


                var currentUser = _httpContextAccessor.HttpContext.User;

                /*
                bool isAddressUpdate = false;
                
                if (RoleHelper.UserHasAnyRole(currentUser, Roles.SuperAdmin, Roles.LawFirm))
                    isAddressUpdate = true;

                */
                // Check if user exists
                if (user != null)
                {
                    user.Email = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    // user.Address = user.Address;//isAddressUpdate == true ? model.Address : string.Empty;
                    user.PhoneNumber = model.PhoneNumber;
                    user.ProfileImage = !string.IsNullOrEmpty(profileImageName) ? profileImageName : user.ProfileImage;
                    user.UpdatedDate = CBSHelpers.CurrentLocalDateTime();
                    _baseDbContext.Update(user);


                    // Create Audit log
                    if (user != null)
                        await _activityLogService.AuditLogsAsync(false, user.Id);

                    await _baseDbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }


        /// <summary>
        /// Get Staff List
        /// </summary>
        /// <param name="dataTableParamsVM"></param>
        /// <returns></returns>
        public async Task<List<LawFirmStaffVM>> GetStaffList(DataTableParamsVM dataTableParamsVM)
        {
            List<LawFirmStaffVM> _lawFirmStaffDetails = new List<LawFirmStaffVM>();

            // Check datatable pageSize
            if (dataTableParamsVM.pageSize > 0)
            {
                _lawFirmStaffDetails = await _baseDbContext.Users
                   .Join(
                   _baseDbContext.UserRoles,
                   u => u.Id,
                   ur => ur.UserId,
                   (u, ur) => new { U = u, UR = ur })
                   .Join(
                   _baseDbContext.Roles.Where(z => z.Prefix == RolesPrefix.CBSStaffPrefix || z.Prefix == RolesPrefix.CBSAdminPrefix),
                   urr => urr.UR.RoleId,
                   r => r.Id,
                   (urr, r) => new { URR = urr, R = r }
                   ).Skip(dataTableParamsVM.skip).Take(dataTableParamsVM.pageSize).Select(x => new LawFirmStaffVM
                   {
                       Id = x.URR.U.Id,
                       CustomUserId = x.URR.U.CustomUserId,
                       FirstName = x.URR.U.FirstName,
                       LastName = x.URR.U.LastName,
                       Role = x.R.Name,
                       Email = x.URR.U.Email,
                       AddedOn = x.URR.U.CreatedDate
                       // DisplayDate = x.URR.U.CreatedDate.ToDateWithMMDDYYYY(),
                   }).ToListAsync();
            }
            else
            {

                _lawFirmStaffDetails = await _baseDbContext.Users
                  .Join(
                  _baseDbContext.UserRoles,
                  u => u.Id,
                  ur => ur.UserId,
                  (u, ur) => new { U = u, UR = ur })
                  .Join(
                  _baseDbContext.Roles.Where(z => z.Prefix == RolesPrefix.CBSStaffPrefix || z.Prefix == RolesPrefix.CBSAdminPrefix),
                  urr => urr.UR.RoleId,
                  r => r.Id,
                  (urr, r) => new { URR = urr, R = r }
                  ).Select(x => new LawFirmStaffVM
                  {
                      Id = x.URR.U.Id,
                      CustomUserId = x.URR.U.CustomUserId,
                      FirstName = x.URR.U.FirstName,
                      LastName = x.URR.U.LastName,
                      Role = x.R.Name,
                      Email = x.URR.U.Email,
                      AddedOn = x.URR.U.CreatedDate
                      // DisplayDate = x.URR.U.CreatedDate.ToDateWithMMDDYYYY(),
                  }).ToListAsync();
            }
            if (!(string.IsNullOrEmpty(dataTableParamsVM.sortColumn) && string.IsNullOrEmpty(dataTableParamsVM.sortColumnDirection)))
            {
                if (dataTableParamsVM.sortColumnDirection == SortDirection.Asc)
                {
                    switch (dataTableParamsVM.sortColumn)
                    {
                        case "id":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderBy(x => x.Id).ToList();
                            break;
                        case "customUserId":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderBy(x => x.CustomUserId).ToList();
                            break;
                        case "firstName":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderBy(x => x.FirstName).ToList();
                            break;
                        case "lastName":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderBy(x => x.LastName).ToList();
                            break;
                        case "role":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderBy(x => x.Role).ToList();
                            break;
                        case "email":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderBy(x => x.Email).ToList();
                            break;
                        case "addedOn":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderBy(x => x.AddedOn).ToList();
                            break;
                        default:
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderBy(x => x.AddedOn).ToList();
                            break;
                    }
                }
                else
                {
                    switch (dataTableParamsVM.sortColumn)
                    {
                        case "id":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderByDescending(x => x.Id).ToList();
                            break;
                        case "customUserId":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderByDescending(x => x.CustomUserId).ToList();
                            break;
                        case "firstName":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderByDescending(x => x.FirstName).ToList();
                            break;
                        case "lastName":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderByDescending(x => x.LastName).ToList();
                            break;
                        case "role":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderByDescending(x => x.Role).ToList();
                            break;
                        case "email":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderByDescending(x => x.Email).ToList();
                            break;
                        case "addedOn":
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderByDescending(x => x.AddedOn).ToList();
                            break;
                        default:
                            _lawFirmStaffDetails = _lawFirmStaffDetails.OrderByDescending(x => x.AddedOn).ToList();
                            break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(dataTableParamsVM.searchValue))
            {
                _lawFirmStaffDetails = _lawFirmStaffDetails.Where(m => m.Id.ToString().Contains(dataTableParamsVM.searchValue)
                                            || m.CustomUserId.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.FirstName.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.LastName.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.Role.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.Email.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.AddedOn.ToString().Contains(dataTableParamsVM.searchValue)).ToList();
            }
            dataTableParamsVM.recordsTotal = _baseDbContext.Users
                .Join(
                _baseDbContext.UserRoles,
                u => u.Id,
                ur => ur.UserId,
                (u, ur) => new { U = u, UR = ur })
                .Join(
                _baseDbContext.Roles.Where(z => z.Prefix == RolesPrefix.CBSStaffPrefix || z.Prefix == RolesPrefix.CBSAdminPrefix),
                urr => urr.UR.RoleId,
                r => r.Id,
                (urr, r) => new { URR = urr, R = r }
                ).Count();
            return _lawFirmStaffDetails;
        }


        /// <summary>
        /// Get user role by userId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RolesViewModel GetUserRoleByUserId(string id)
        {
            string roleName = string.Empty;
            var userRoleDetails = _baseDbContext.UserRoles.Where(x => x.UserId == id).FirstOrDefault();
            if (userRoleDetails != null)
            {
                return _baseDbContext.Roles.Where(x => x.Id == userRoleDetails.RoleId).Select(x => new RolesViewModel { Name = x.Name, DisplayName = x.DisplayName }).FirstOrDefault();
            }
            else
            {
                return new RolesViewModel();
            }
        }


        /// <summary>
        ///  Activate/de-activate user
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ActiveDeactiveUserAsync(string id, bool isActive, string loggedInUserId)
        {

            // Get logged In user role
            var loggedInUserRole = GetUserRoleByUserId(loggedInUserId);

            var userDetail = await _baseDbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (userDetail != null)
            {
                userDetail.IsActive = isActive;
            }
            _baseDbContext.Update(userDetail);

            // Create Audit log
            if (userDetail != null)
                await _activityLogService.AuditLogsAsync(false, userDetail.Id);

            await _baseDbContext.SaveChangesAsync();

            // Create Notification
            NotificationViewModel notification = new NotificationViewModel()
            {
                TriggerByUserId = loggedInUserId,
                TriggerForUserId = loggedInUserRole.Name == Roles.SuperAdmin ? loggedInUserId : null,
                SourceType = (int)SourceType.ActivityLog,
                Type = isActive == true ? (int)NotificationType.UserActivated : (int)NotificationType.UserDeActivated,
                PatientOrUserId = id
            };
            _notificationService.CreateNotification(notification, loggedInUserId);


            return true;
        }


        /// <summary>
        /// Get Menu on the basis of role
        /// </summary>
        /// <param name="cBSAdmin"></param>
        /// <returns></returns>
        public async Task<PermissionsVM> GetMenuConfigByUserRole(string cBSAdmin)
        {
            PermissionsVM permissionsVM = new PermissionsVM();
            var role = await _baseDbContext.Roles.Where(x => x.Name == cBSAdmin).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(role.Id))
            {
                var rolePermission = await _baseDbContext.RolePermissions.Where(x => x.RoleId == role.Id)
                    .Join(_baseDbContext.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => new { RP = rp, P = p }).Select(x => new
                    {
                        x.P.Id,
                        x.P.Name

                    }).ToListAsync();
                permissionsVM.LawFirmManagement = rolePermission.Where(x => x.Name == Permissions.LawFirmManagement).FirstOrDefault() != null ? true : false;
                permissionsVM.DataDownloadRequest = rolePermission.Where(x => x.Name == Permissions.DataDownloadRequest).FirstOrDefault() != null ? true : false;
                permissionsVM.PatientManagement = rolePermission.Where(x => x.Name == Permissions.PatientManagement).FirstOrDefault() != null ? true : false;
                permissionsVM.StaffManagement = rolePermission.Where(x => x.Name == Permissions.StaffManagement).FirstOrDefault() != null ? true : false;
                permissionsVM.ActivityLogs = rolePermission.Where(x => x.Name == Permissions.ActivityLogs).FirstOrDefault() != null ? true : false;
            }
            return permissionsVM;
        }
    }
}
