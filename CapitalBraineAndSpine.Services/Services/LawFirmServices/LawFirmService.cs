using CapitalBraineAndSpine.Data.Contexts;
using CapitalBraineAndSpine.Data.Entities.IdentityEntities;
using CapitalBraineAndSpine.Data.Entities.RoleAndPermissionEntities;
using CapitalBraineAndSpine.Service.Infrastructures.ExtensionHelper;
using CapitalBraineAndSpine.Service.Infrastructures.Helpers;
using CapitalBraineAndSpine.Services.Infrastructures.Helpers;
using CapitalBraineAndSpine.Services.Services.Abstracts.AuditLogAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.EmailAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.LawFirmAbstracts;
using CapitalBraineAndSpine.Services.ViewModels.AccountViewModels;
using CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels;
using CapitalBraineAndSpine.Services.ViewModels.LawFirmViewModels;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;
namespace CapitalBraineAndSpine.Services.Services.LawFirmServices
{
    public class LawFirmService : ILawFirmService
    {
        private readonly BaseDbContext _baseDbContext;
        private readonly IEmailService _emailService;
        private readonly UserManager<CBSUser> _userManager;
        private readonly RoleManager<CBSRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogService _activityLogService;

        public LawFirmService(BaseDbContext baseDbContext
            , IEmailService emailService
            , UserManager<CBSUser> userManager
            , RoleManager<CBSRole> roleManager
            , IConfiguration configuration, IHttpContextAccessor httpContextAccessor
            , IAuditLogService activityLogService
            )
        {
            _baseDbContext = baseDbContext;
            _emailService = emailService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _activityLogService = activityLogService;
        }

        public async Task<RegisterViewModel> ManageLawFirmAsync(RegisterViewModel model)
        {
            var user = new CBSUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Zip = model.Zip,
                PhoneNumber = model.PhoneNumber,
                CreatedDate = CBSHelpers.CurrentLocalDateTime(),
                UpdatedDate = CBSHelpers.CurrentLocalDateTime()
                //UserCreatedBy = (await GetCurrentUserAsync()).Id,
            };

            // Create user in database
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Add role
                var lawFirmRole = new CBSRole { Name = Roles.LawFirm, Caption = Roles.LawFirm, Prefix = RolesPrefix.LawFirmPrefix };


                // Create role is not exists in Database
                if (!await _roleManager.RoleExistsAsync(lawFirmRole.Name))
                    await _roleManager.CreateAsync(lawFirmRole);

                // Assign role to user if not assigned
                if (!await _userManager.IsInRoleAsync(user, lawFirmRole.Name))
                    await _userManager.AddToRoleAsync(user, lawFirmRole.Name);


                // Get user details
                var userDetails = _baseDbContext.Users.Where(x => x.Id == user.Id).FirstOrDefault();

                // Check user details available
                if (userDetails != null)
                {
                    // Get user role
                    var userRole = await _userManager.GetRolesAsync(user);

                    // Get Max Prefix Values of role from users
                    var usersInRole = await _userManager.GetUsersInRoleAsync(lawFirmRole.Name);

                    // Get max id
                    var maxPrefixId = usersInRole.OrderByDescending(x => x.PrefixVal).Max(x => x.PrefixVal);

                    // Create customUserId
                    var customUserId = lawFirmRole != null ? $"{lawFirmRole.Prefix}{(maxPrefixId > 0 ? maxPrefixId + 1 : 1)}" : string.Empty;

                    userDetails.CustomUserId = customUserId;

                    // Set prefix to identity user
                    userDetails.PrefixVal = maxPrefixId > 0 ? maxPrefixId + 1 : 1;

                    _baseDbContext.Users.Update(userDetails);
                    await _baseDbContext.SaveChangesAsync();

                    // Base url from AppSettings
                    var baseUrl = Convert.ToString(_configuration["WebsiteUrl:url"]);
                    // Send template                        
                    var tbl = new Hashtable
                        {
                            { "@FirstName", model.FirstName },
                            { "@LastName", model.LastName },
                            { "@Username", model.Email },
                            { "@baseurl", $"{baseUrl}account/login" }
                        };

                    // Get html for Email
                    string bodyContent = _emailService.GetMailBody("NotifyUser.html", tbl);

                    EmailViewModel emailModel = new EmailViewModel { To = user.Email, Subject = "Your login details", Body = bodyContent };

                    await _emailService.SendEmailAsync(emailModel);
                }

            }

            return model;
        }


        public async Task<List<RolePermissionVM>> GetRolePermissions(string targetRole)
        {
            // Get role and permission
            var rolePermissions = await _baseDbContext.RolePermissions
                .Select(g => new RolePermissionVM
                {
                    RoleId = g.RoleId,
                    PermissionId = g.PermissionId,
                    IsSelected = true
                }).ToListAsync();

            // Get roles
            var rolesPermissionjoin = await _baseDbContext.Roles.SelectMany(r => _baseDbContext.Permissions.Select(p => new
            RolePermissionVM
            {
                RoleId = r.Id,
                RoleName = r.Name,
                PermissionId = p.Id,
                PermissionName = p.Name,
                IsSelected = false
            })).ToListAsync();

            // Get role's permission on the basis of Role
            if (targetRole == Roles.SuperAdmin)
            {
                rolesPermissionjoin = rolesPermissionjoin.Where(x => x.RoleName == Roles.CBSStaff && x.PermissionName != Permissions.StaffManagement).OrderBy(x => x.RoleName).ToList();
            }
            else if (targetRole == Roles.LawFirm)
            {
                rolesPermissionjoin = rolesPermissionjoin.Where(x => x.PermissionName != Permissions.LawFirmManagement && (x.RoleName == Roles.LawFirm || x.RoleName == Roles.Attorney)).OrderBy(x => x.RoleName).ToList();
                var rolesPermissionjoinAttr = rolesPermissionjoin.Where(x => x.RoleName == Roles.Attorney && x.PermissionName == Permissions.StaffManagement).OrderBy(x => x.RoleName).FirstOrDefault();
                rolesPermissionjoin.Remove(rolesPermissionjoinAttr);
            }
            else if (targetRole == Roles.Attorney)
            {
                rolesPermissionjoin = rolesPermissionjoin.Where(x => x.RoleName == Roles.Attorney && (x.PermissionName == Permissions.ActivityLogs || x.PermissionName == Permissions.PatientManagement || x.PermissionName == Permissions.DataDownloadRequest)).OrderBy(x => x.RoleName).ToList();
            }

            if (rolePermissions.Count > 0)
            {
                foreach (var role in rolePermissions)
                {
                    var rolefound = rolesPermissionjoin.FirstOrDefault(x => x.RoleId == role.RoleId && x.PermissionId == role.PermissionId);
                    if (rolefound != null) rolefound.IsSelected = role.IsSelected;
                }
            }
            return rolesPermissionjoin;
        }

        public async Task UpdatePermissionsAsync(RolePermissionViewModel rolePermissionVMs)
        {
            List<string> roleIds = new List<string>();
            if (rolePermissionVMs.TargetRole == Roles.SuperAdmin)
            {
                roleIds = _baseDbContext.Roles.Where(x => x.Name == Roles.CBSStaff).Select(x => x.Id).ToList();
            }
            else if (rolePermissionVMs.TargetRole == Roles.LawFirm)
            {
                roleIds = _baseDbContext.Roles.Where(x => x.Name == Roles.LawFirm || x.Name == Roles.Attorney).Select(x => x.Id).ToList();
            }
            else if (rolePermissionVMs.TargetRole == Roles.Attorney)
            {
                roleIds = _baseDbContext.Roles.Where(x => x.Name == Roles.Attorney).Select(x => x.Id).ToList();
            }

            if (roleIds.Count > 0)
            {
                // Remove existing configuration
                foreach (var roleId in roleIds)
                {
                    var existingPermission = await _baseDbContext.RolePermissions.Where(x => x.RoleId == roleId).ToListAsync();
                    _baseDbContext.RolePermissions.RemoveRange(existingPermission);
                    _baseDbContext.SaveChanges();
                }
                foreach (var rolePermission in rolePermissionVMs.RolePermissions)
                {
                    // Add new permissions
                    var config = await _baseDbContext.RolePermissions.Where(rp => rp.RoleId == rolePermission.RoleId && rp.PermissionId == rolePermission.PermissionId).SingleOrDefaultAsync();
                    if (config == null && rolePermission.IsSelected)
                    {
                        RolePermission role_Permission = new RolePermission
                        {
                            RoleId = rolePermission.RoleId,
                            PermissionId = rolePermission.PermissionId,
                            CreatedDate = CBSHelpers.CurrentLocalDateTime(),
                            LastUpdated = CBSHelpers.CurrentLocalDateTime()
                        };
                        _baseDbContext.RolePermissions.Add(role_Permission);
                        _baseDbContext.SaveChanges();
                    }


                }
            }
        }

        public async Task<List<LawFirmVM>> GetLawFirmDetails(DataTableParamsVM dataTableParamsVM)
        {
            List<LawFirmVM> _lawFirmDetails = new List<LawFirmVM>();
            if (dataTableParamsVM.pageSize > 0)
            {
                _lawFirmDetails = await _baseDbContext.Users
                    .Join(
                    _baseDbContext.UserRoles,
                    u => u.Id,
                    ur => ur.UserId,
                    (u, ur) => new { U = u, UR = ur })
                    .Join(
                    _baseDbContext.Roles.Where(z => z.Prefix == RolesPrefix.LawFirmPrefix),
                    urr => urr.UR.RoleId,
                    r => r.Id,
                    (urr, r) => new { URR = urr, R = r }
                    ).Skip(dataTableParamsVM.skip).Take(dataTableParamsVM.pageSize).Select(x => new LawFirmVM
                    {
                        Id = x.URR.U.Id,
                        CustomUserId = x.URR.U.CustomUserId,
                        FirstName = x.URR.U.FirstName,
                        LastName = x.URR.U.LastName,
                        LawFirmName = x.URR.U.LawFirmName != null ? x.URR.U.LawFirmName : string.Empty,
                        PhoneNumber = x.URR.U.PhoneNumber,
                        Email = x.URR.U.Email,
                        DisplayDate = x.URR.U.CreatedDate.ToDateWithMMDDYYYY(),
                        CreatedDate = x.URR.U.CreatedDate,
                        DownloadRequest = _baseDbContext.PatientDataRequest.Count(y => y.LawFirmUserId == x.URR.U.Id)
                    }).ToListAsync();
            }
            else
            {
                _lawFirmDetails = await _baseDbContext.Users
                    .Join(
                    _baseDbContext.UserRoles,
                    u => u.Id,
                    ur => ur.UserId,
                    (u, ur) => new { U = u, UR = ur })
                    .Join(
                    _baseDbContext.Roles.Where(z => z.Prefix == RolesPrefix.LawFirmPrefix),
                    urr => urr.UR.RoleId,
                    r => r.Id,
                    (urr, r) => new { URR = urr, R = r }
                    ).Select(x => new LawFirmVM
                    {
                        Id = x.URR.U.Id,
                        CustomUserId = x.URR.U.CustomUserId,
                        FirstName = x.URR.U.FirstName,
                        LastName = x.URR.U.LastName,
                        LawFirmName = x.URR.U.LawFirmName != null ? x.URR.U.LawFirmName : string.Empty,
                        PhoneNumber = x.URR.U.PhoneNumber,
                        Email = x.URR.U.Email,
                        DisplayDate = x.URR.U.CreatedDate.ToDateWithMMDDYYYY(),
                        CreatedDate = x.URR.U.CreatedDate,
                        DownloadRequest = _baseDbContext.PatientDataRequest.Count(y => y.LawFirmUserId == x.URR.U.Id)
                    }).ToListAsync();
            }

            if (!string.IsNullOrEmpty(dataTableParamsVM.searchValue))
            {
                _lawFirmDetails = _lawFirmDetails.Where(m => m.Id.ToString().Contains(dataTableParamsVM.searchValue)
                                            || m.CustomUserId.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.FirstName.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.LastName.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.LawFirmName.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.PhoneNumber.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.Email.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.DownloadRequest.ToString().Contains(dataTableParamsVM.searchValue)).ToList();
            }

            if (!(string.IsNullOrEmpty(dataTableParamsVM.sortColumn) && string.IsNullOrEmpty(dataTableParamsVM.sortColumnDirection)))
            {
                if (dataTableParamsVM.sortColumnDirection == SortDirection.Asc)
                {
                    switch (dataTableParamsVM.sortColumn)
                    {
                        case "id":
                            _lawFirmDetails = _lawFirmDetails.OrderBy(x => x.Id).ToList();
                            break;
                        case "customUserId":
                            _lawFirmDetails = _lawFirmDetails.OrderBy(x => x.CustomUserId).ToList();
                            break;
                        case "firstName":
                            _lawFirmDetails = _lawFirmDetails.OrderBy(x => x.FirstName).ToList();
                            break;
                        case "lastName":
                            _lawFirmDetails = _lawFirmDetails.OrderBy(x => x.LastName).ToList();
                            break;
                        case "lawFirmName":
                            _lawFirmDetails = _lawFirmDetails.OrderBy(x => x.LawFirmName).ToList();
                            break;
                        case "phoneNumber":
                            _lawFirmDetails = _lawFirmDetails.OrderBy(x => x.PhoneNumber).ToList();
                            break;
                        case "email":
                            _lawFirmDetails = _lawFirmDetails.OrderBy(x => x.Email).ToList();
                            break;
                        case "downloadRequest":
                            _lawFirmDetails = _lawFirmDetails.OrderBy(x => x.DownloadRequest).ToList();
                            break;
                        case "displayDate":
                            _lawFirmDetails = _lawFirmDetails.OrderBy(x => x.DisplayDate).ToList();
                            break;
                        default:
                            _lawFirmDetails = _lawFirmDetails.OrderBy(x => x.DisplayDate).ToList();
                            break;
                    }
                }
                else
                {
                    switch (dataTableParamsVM.sortColumn)
                    {
                        case "id":
                            _lawFirmDetails = _lawFirmDetails.OrderByDescending(x => x.Id).ToList();
                            break;
                        case "customUserId":
                            _lawFirmDetails = _lawFirmDetails.OrderByDescending(x => x.CustomUserId).ToList();
                            break;
                        case "firstName":
                            _lawFirmDetails = _lawFirmDetails.OrderByDescending(x => x.FirstName).ToList();
                            break;
                        case "lastName":
                            _lawFirmDetails = _lawFirmDetails.OrderByDescending(x => x.LastName).ToList();
                            break;
                        case "lawFirmName":
                            _lawFirmDetails = _lawFirmDetails.OrderByDescending(x => x.LawFirmName).ToList();
                            break;
                        case "phoneNumber":
                            _lawFirmDetails = _lawFirmDetails.OrderByDescending(x => x.PhoneNumber).ToList();
                            break;
                        case "email":
                            _lawFirmDetails = _lawFirmDetails.OrderByDescending(x => x.Email).ToList();
                            break;
                        case "downloadRequest":
                            _lawFirmDetails = _lawFirmDetails.OrderByDescending(x => x.DownloadRequest).ToList();
                            break;
                        case "displayDate":
                            _lawFirmDetails = _lawFirmDetails.OrderBy(x => x.DisplayDate).ToList();
                            break;
                        default:
                            _lawFirmDetails = _lawFirmDetails.OrderByDescending(x => x.CreatedDate).ToList();
                            break;
                    }
                }
            }

            dataTableParamsVM.recordsTotal = _baseDbContext.Users
                .Join(
                _baseDbContext.UserRoles,
                u => u.Id,
                ur => ur.UserId,
                (u, ur) => new { U = u, UR = ur })
                .Join(
                _baseDbContext.Roles.Where(z => z.Prefix == RolesPrefix.LawFirmPrefix),
                urr => urr.UR.RoleId,
                r => r.Id,
                (urr, r) => new { URR = urr, R = r }
                ).Count();
            return _lawFirmDetails;
        }

        public async Task<List<LawFirmStaffVM>> GetLawFirmStaffDetails(DataTableParamsVM dataTableParamsVM)
        {
            // Get logged-In user
            var user = _httpContextAccessor.HttpContext.User;

            var _lawFirmStaffDetails = new List<LawFirmStaffVM>();

            if (RoleHelper.UserHasAnyRole(user, Roles.SuperAdmin, Roles.CBSAdmin, Roles.CBSStaff))
            {
                if (dataTableParamsVM.pageSize > 0)
                {
                    _lawFirmStaffDetails = await _baseDbContext.Users.Where(u => u.ParentUserId == dataTableParamsVM.userId)
                       .Join(
                       _baseDbContext.UserRoles,
                       u => u.Id,
                       ur => ur.UserId,
                       (u, ur) => new { U = u, UR = ur })
                       .Join(
                       _baseDbContext.Roles.Where(z => z.Prefix == RolesPrefix.AttorneyPrefix || z.Prefix == RolesPrefix.LFStaffPrefix),
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
                           AddedOn = x.URR.U.CreatedDate,
                           ParentUserId = x.URR.U.ParentUserId,
                           IsActive = x.URR.U.IsActive,
                           //DisplayDate = x.URR.U.CreatedDate.ToDateWithMMDDYYYY()

                       }).ToListAsync();

                    dataTableParamsVM.recordsTotal = _lawFirmStaffDetails.Count();

                    _lawFirmStaffDetails = _lawFirmStaffDetails.Skip(dataTableParamsVM.skip).Take(dataTableParamsVM.pageSize).ToList();



                }
                else
                {
                    _lawFirmStaffDetails = await _baseDbContext.Users.Where(u => u.ParentUserId == dataTableParamsVM.userId)
                      .Join(
                      _baseDbContext.UserRoles,
                      u => u.Id,
                      ur => ur.UserId,
                      (u, ur) => new { U = u, UR = ur })
                      .Join(
                      _baseDbContext.Roles.Where(z => z.Prefix == RolesPrefix.AttorneyPrefix || z.Prefix == RolesPrefix.LFStaffPrefix),
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
                          AddedOn = x.URR.U.CreatedDate,
                          ParentUserId = x.URR.U.ParentUserId,
                          IsActive = x.URR.U.IsActive,
                          //DisplayDate = x.URR.U.CreatedDate.ToDateWithMMDDYYYY()

                      }).ToListAsync();

                    dataTableParamsVM.recordsTotal = _lawFirmStaffDetails.Count();
                }

            }
            else if (RoleHelper.UserHasAnyRole(user, Roles.LawFirm, Roles.Attorney))
            {
                if (RoleHelper.UserHasAnyRole(user, Roles.Attorney))
                {
                    dataTableParamsVM.userId = _baseDbContext.Users.Where(x => x.Id == dataTableParamsVM.userId).FirstOrDefault().ParentUserId;
                }
                if (dataTableParamsVM.pageSize > 0)
                {
                    _lawFirmStaffDetails = await _baseDbContext.Users.Where(x => x.ParentUserId == dataTableParamsVM.userId)
                       .Join(
                       _baseDbContext.UserRoles,
                       u => u.Id,
                       ur => ur.UserId,
                       (u, ur) => new { U = u, UR = ur })
                       .Join(
                       _baseDbContext.Roles.Where(z => z.Prefix == RolesPrefix.AttorneyPrefix || z.Prefix == RolesPrefix.LFStaffPrefix),
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
                           AddedOn = x.URR.U.CreatedDate,
                           ParentUserId = x.URR.U.ParentUserId,
                           IsActive = x.URR.U.IsActive,
                           //DisplayDate = x.URR.U.CreatedDate.ToDateWithMMDDYYYY()
                       }).ToListAsync();

                    dataTableParamsVM.recordsTotal = _lawFirmStaffDetails.Count();

                    _lawFirmStaffDetails = _lawFirmStaffDetails.Skip(dataTableParamsVM.skip).Take(dataTableParamsVM.pageSize).ToList();




                }
                else
                {
                    _lawFirmStaffDetails = await _baseDbContext.Users.Where(x => x.ParentUserId == dataTableParamsVM.userId)
                      .Join(
                      _baseDbContext.UserRoles,
                      u => u.Id,
                      ur => ur.UserId,
                      (u, ur) => new { U = u, UR = ur })
                      .Join(
                      _baseDbContext.Roles.Where(z => z.Prefix == RolesPrefix.AttorneyPrefix || z.Prefix == RolesPrefix.LFStaffPrefix),
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
                          AddedOn = x.URR.U.CreatedDate,
                          ParentUserId = x.URR.U.ParentUserId,
                          IsActive = x.URR.U.IsActive,
                          // DisplayDate = x.URR.U.CreatedDate.ToDateWithMMDDYYYY()
                      }).ToListAsync();

                    dataTableParamsVM.recordsTotal = _lawFirmStaffDetails.Count();
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


            return _lawFirmStaffDetails;
        }


    }
}
