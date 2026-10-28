using AutoMapper;
using CapitalBraineAndSpine.Data.Contexts;
using CapitalBraineAndSpine.Data.Entities.LogsEntities;
using CapitalBraineAndSpine.Service.Infrastructures.ExtensionHelper;
using CapitalBraineAndSpine.Service.Infrastructures.Helpers;
using CapitalBraineAndSpine.Services.Infrastructures.Helpers;
using CapitalBraineAndSpine.Services.Services.Abstracts.AuditLogAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.CommonAbstracts;
using CapitalBraineAndSpine.Services.ViewModels.AuditLogViewModels;
using CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace CapitalBraineAndSpine.Services.Services.ActivityLogServices
{
    public class AuditLogService : IAuditLogService
    {

        private readonly BaseDbContext _baseDbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ICommonService _commonService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(BaseDbContext baseDbContext, IMapper mapper,
             IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _baseDbContext = baseDbContext;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<bool> AuditLogsAsync(bool isNew = false, string userId = "", string userRole = "")
        {
            var changedEntries = _baseDbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

            foreach (var entry in changedEntries)
            {
                var trackChanges = TrackChangesHelper.GetChanges(entry);
                var auditLog = new AuditLogs
                {
                    UserId = userId,      // New/Modified user's UserId
                    UserRole = userRole,  // New/Modified user's UserRole
                    Activity = isNew ? "Created" : string.IsNullOrEmpty(trackChanges) ? "View" : entry.State.ToString(), // "Added", "Modified", or "Deleted"
                    EntityName = entry.Entity.GetType().Name,
                    IpAddress = IpHelper.GetClientIp(),
                    ChangeDetails = isNew == true ? string.Empty : trackChanges, // Change details will be empty if new user created
                    CreatedByRole = GetLoggedInRole(),            // Role of the Action perform by the user
                    CreatedByUserId = GetLoggedInUserId(),          // User Id of the Action perform by the user  
                    Timestamp = CBSHelpers.CurrentLocalDateTime()
                };

                await _baseDbContext.AddAsync(auditLog);
            }

            return true;
        }

        public bool AuditLogsAsync(string patientName, string patientId, string dob, string activity)
        {
            var auditLog = new AuditLogs
            {
                UserId = string.Empty,      // New/Modified user's UserId
                UserRole = string.Empty,  // New/Modified user's UserRole
                Activity = activity,
                EntityName = "Patient",
                IpAddress = IpHelper.GetClientIp(),
                ChangeDetails = !string.IsNullOrEmpty(patientId) ? $"{patientId}\r{patientName}\r{dob}" : $"{patientName}\r{dob}", // Change details will be empty if new user created
                CreatedByRole = GetLoggedInRole(),            // Role of the Action perform by the user
                CreatedByUserId = GetLoggedInUserId(),          // User Id of the Action perform by the user  
                Timestamp = CBSHelpers.CurrentLocalDateTime()
            };

            _baseDbContext.Add(auditLog);
            _baseDbContext.SaveChanges();
            return true;
        }
        public async Task<bool> AuditLogsAsync(string userId, string activity)
        {
            var auditLog = new AuditLogs
            {
                UserId = string.Empty,      // New/Modified user's UserId
                UserRole = string.Empty,  // New/Modified user's UserRole
                Activity = activity,
                EntityName = "User",
                IpAddress = IpHelper.GetClientIp(),
                ChangeDetails = activity,
                CreatedByRole = GetLoggedInRole(),            // Role of the Action perform by the user
                CreatedByUserId = GetLoggedInUserId(),          // User Id of the Action perform by the user  
                Timestamp = CBSHelpers.CurrentLocalDateTime()
            };

            _baseDbContext.Add(auditLog);
            await _baseDbContext.SaveChangesAsync();
            return true;
        }
        private static string GetChangeDetails(string activity, string changeDetails)
        {
            switch (activity)
            {
                case "Modified":
                case "Search":
                case "Request for access":
                case "Request Approved":
                case "Request Rejected":
                case "View Patient Details":
                    return changeDetails.Replace("\r", "<br />");
                case "View":
                    return string.Empty;
                default:
                    return "";
            }
        }

        public async Task<List<AuditLogViewItem>> GetActivityLogsByUserRole(DataTableParamsVM dataTableParamsVM)
        {
            // Get logged In user details
            var user = _httpContextAccessor.HttpContext.User;



            List<AuditLogViewItem> _activityLogViewModelList = new List<AuditLogViewItem>();

            if (dataTableParamsVM.extraParam == "true")
            {
                if (dataTableParamsVM.pageSize > 0)
                {

                    _activityLogViewModelList = await (
                        from al in _baseDbContext.AuditLogs
                        join cu in _baseDbContext.Users on al.CreatedByUserId equals cu.Id into cuJoin
                        from cu in cuJoin.DefaultIfEmpty()
                        join u in _baseDbContext.Users on al.UserId equals u.Id into uJoin
                        from u in uJoin.DefaultIfEmpty()
                        join r in _baseDbContext.Roles on al.CreatedByRole equals r.Name into rJoin
                        from r in rJoin.DefaultIfEmpty()
                        where r.Name == Roles.SuperAdmin
                           || r.Name == Roles.CBSStaff
                           || r.Name == Roles.CBSAdmin
                        orderby al.Timestamp descending

                        select new AuditLogViewItem
                        {
                            Id = al.Id,
                            UserId = al.CreatedByUserId,
                            UserName = $"{u.FirstName} {u.LastName}",
                            CreatedByRole = al.CreatedByRole,
                            CreateByUserName = al.CreatedByUserId == dataTableParamsVM.userId ? "You" : $"{cu.FirstName} {cu.LastName}",
                            UserRole = al.UserRole,
                            Timestamp = al.Timestamp,
                            Activity = al.Activity,
                            ChangeDetails = GetChangeDetails(al.Activity, al.ChangeDetails),
                            DisplayDate = al.Timestamp.ToDateWithMMDDYYYY()
                        })
                        .Skip(dataTableParamsVM.skip)
                        .Take(dataTableParamsVM.pageSize)
                        .ToListAsync();


                    dataTableParamsVM.recordsTotal = await (
                    from al in _baseDbContext.AuditLogs
                    join cu in _baseDbContext.Users on al.CreatedByUserId equals cu.Id into cuJoin
                    from cu in cuJoin.DefaultIfEmpty()
                    join u in _baseDbContext.Users on al.UserId equals u.Id into uJoin
                    from u in uJoin.DefaultIfEmpty()
                    join r in _baseDbContext.Roles on al.CreatedByRole equals r.Name into rJoin
                    from r in rJoin.DefaultIfEmpty()
                    where (r.Name == Roles.SuperAdmin
                    || r.Name == Roles.CBSStaff
                    || r.Name == Roles.CBSAdmin)
                    select al).CountAsync();
                }
            }
            else
            if (RoleHelper.UserHasAnyRole(user, Roles.SuperAdmin, Roles.CBSAdmin, Roles.CBSStaff))
            {
                if (dataTableParamsVM.pageSize > 0)
                {


                    _activityLogViewModelList = await (
                   from al in _baseDbContext.AuditLogs
                   join cu in _baseDbContext.Users on al.CreatedByUserId equals cu.Id into cuJoin
                   from cu in cuJoin.DefaultIfEmpty()
                   join u in _baseDbContext.Users on al.UserId equals u.Id into uJoin
                   from u in uJoin.DefaultIfEmpty()
                   join r in _baseDbContext.Roles on al.CreatedByRole equals r.Name into rJoin
                   from r in rJoin.DefaultIfEmpty()
                   orderby (al.Timestamp) descending

                   select new AuditLogViewItem
                   {
                       Id = al.Id,
                       UserId = al.UserId,
                       UserName = $"{u.FirstName} {u.LastName}",
                       CreatedByRole = al.CreatedByRole,
                       CreateByUserName = al.CreatedByUserId == dataTableParamsVM.userId ? "You" : $"{cu.FirstName} {cu.LastName}",
                       UserRole = al.UserRole,
                       Timestamp = al.Timestamp,
                       Activity = al.Activity,
                       ChangeDetails = GetChangeDetails(al.Activity, al.ChangeDetails),
                       DisplayDate = al.Timestamp.ToDateWithMMDDYYYY()
                   })
                        .Skip(dataTableParamsVM.skip)
                        .Take(dataTableParamsVM.pageSize)
                        .ToListAsync();

                }
                else
                {


                    _activityLogViewModelList = await (
                    from al in _baseDbContext.AuditLogs
                    join cu in _baseDbContext.Users on al.CreatedByUserId equals cu.Id into cuJoin
                    from cu in cuJoin.DefaultIfEmpty()
                    join u in _baseDbContext.Users on al.UserId equals u.Id into uJoin
                    from u in uJoin.DefaultIfEmpty()
                    join r in _baseDbContext.Roles on al.CreatedByRole equals r.Name into rJoin
                    from r in rJoin.DefaultIfEmpty()
                    orderby (al.Timestamp) descending

                    select new AuditLogViewItem
                    {
                        Id = al.Id,
                        UserId = al.UserId,
                        UserName = $"{u.FirstName} {u.LastName}",
                        CreatedByRole = al.CreatedByRole,
                        CreateByUserName = al.CreatedByUserId == dataTableParamsVM.userId ? "You" : $"{cu.FirstName} {cu.LastName}",
                        UserRole = al.UserRole,
                        Timestamp = al.Timestamp,
                        Activity = al.Activity,
                        ChangeDetails = GetChangeDetails(al.Activity, al.ChangeDetails),
                        DisplayDate = al.Timestamp.ToDateWithMMDDYYYY()
                    }).ToListAsync();

                }

                // Total record count
                //dataTableParamsVM.recordsTotal = await _baseDbContext.NotificationsAndActivity.Where(x => x.SourceType == (int)SourceType.ActivityLog).CountAsync();

                dataTableParamsVM.recordsTotal = await (
                from al in _baseDbContext.AuditLogs
                join cu in _baseDbContext.Users on al.CreatedByUserId equals cu.Id into cuJoin
                from cu in cuJoin.DefaultIfEmpty()
                join u in _baseDbContext.Users on al.UserId equals u.Id into uJoin
                from u in uJoin.DefaultIfEmpty()
                join r in _baseDbContext.Roles on al.CreatedByRole equals r.Name into rJoin
                from r in rJoin.DefaultIfEmpty()
                select al).CountAsync();
            }
            else if (RoleHelper.UserHasAnyRole(user, Roles.LawFirm))
            {

                if (RoleHelper.UserHasAnyRole(user, Roles.Attorney))
                {
                    dataTableParamsVM.userId = _baseDbContext.Users.Where(x => x.Id == dataTableParamsVM.userId).FirstOrDefault().ParentUserId;
                }
                if (dataTableParamsVM.pageSize > 0)
                {
                    _activityLogViewModelList = await (
                from al in _baseDbContext.AuditLogs
                join cu in _baseDbContext.Users on al.CreatedByUserId equals cu.Id into cuJoin
                from cu in cuJoin.DefaultIfEmpty()
                join u in _baseDbContext.Users on al.UserId equals u.Id into uJoin
                from u in uJoin.DefaultIfEmpty()
                join r in _baseDbContext.Roles on al.CreatedByRole equals r.Name into rJoin
                from r in rJoin.DefaultIfEmpty()
                where al.CreatedByUserId == dataTableParamsVM.userId
                orderby (al.Timestamp) descending

                select new AuditLogViewItem
                {
                    Id = al.Id,
                    UserId = al.UserId,
                    UserName = $"{u.FirstName} {u.LastName}",
                    CreatedByRole = al.CreatedByRole,
                    CreateByUserName = al.CreatedByUserId == dataTableParamsVM.userId ? "You" : $"{cu.FirstName} {cu.LastName}",
                    UserRole = al.UserRole,
                    Timestamp = al.Timestamp,
                    Activity = al.Activity,
                    ChangeDetails = GetChangeDetails(al.Activity, al.ChangeDetails),
                    DisplayDate = al.Timestamp.ToDateWithMMDDYYYY()
                })
                         .Skip(dataTableParamsVM.skip)
                         .Take(dataTableParamsVM.pageSize)
                         .ToListAsync();



                }
                else
                {

                    _activityLogViewModelList = await (
                from al in _baseDbContext.AuditLogs
                join cu in _baseDbContext.Users on al.CreatedByUserId equals cu.Id into cuJoin
                from cu in cuJoin.DefaultIfEmpty()
                join u in _baseDbContext.Users on al.UserId equals u.Id into uJoin
                from u in uJoin.DefaultIfEmpty()
                join r in _baseDbContext.Roles on al.CreatedByRole equals r.Name into rJoin
                from r in rJoin.DefaultIfEmpty()
                where al.CreatedByUserId == dataTableParamsVM.userId
                orderby (al.Timestamp) descending

                select new AuditLogViewItem
                {
                    Id = al.Id,
                    UserId = al.UserId,
                    UserName = $"{u.FirstName} {u.LastName}",
                    CreatedByRole = al.CreatedByRole,
                    CreateByUserName = al.CreatedByUserId == dataTableParamsVM.userId ? "You" : $"{cu.FirstName} {cu.LastName}",
                    UserRole = al.UserRole,
                    Timestamp = al.Timestamp,
                    Activity = al.Activity,
                    ChangeDetails = GetChangeDetails(al.Activity, al.ChangeDetails),
                    DisplayDate = al.Timestamp.ToDateWithMMDDYYYY()
                }).ToListAsync();

                }
                dataTableParamsVM.recordsTotal = await _baseDbContext.AuditLogs.Where(x => x.CreatedByUserId == dataTableParamsVM.userId).CountAsync();
            }



            if (!(string.IsNullOrEmpty(dataTableParamsVM.sortColumn) && string.IsNullOrEmpty(dataTableParamsVM.sortColumnDirection)))
            {
                if (dataTableParamsVM.sortColumnDirection == SortDirection.Asc)
                {
                    switch (dataTableParamsVM.sortColumn)
                    {
                        case "timeStamp":
                            _activityLogViewModelList = _activityLogViewModelList.OrderBy(x => x.Timestamp).ToList();
                            break;
                        case "createByUserName":
                            _activityLogViewModelList = _activityLogViewModelList.OrderBy(x => x.CreateByUserName).ToList();
                            break;
                        case "userRole":
                            _activityLogViewModelList = _activityLogViewModelList.OrderBy(x => x.UserRole).ToList();
                            break;
                        case "changeDetails":
                            _activityLogViewModelList = _activityLogViewModelList.OrderBy(x => x.ChangeDetails).ToList();
                            break;
                        case "activity":
                            _activityLogViewModelList = _activityLogViewModelList.OrderBy(x => x.Activity).ToList();
                            break;
                        case "id":
                            _activityLogViewModelList = _activityLogViewModelList.OrderBy(x => x.Id).ToList();
                            break;
                        default:
                            _activityLogViewModelList = _activityLogViewModelList.OrderBy(x => x.Id).ToList();
                            break;
                    }
                }
                else
                {
                    switch (dataTableParamsVM.sortColumn)
                    {
                        case "timeStamp":
                            _activityLogViewModelList = _activityLogViewModelList.OrderByDescending(x => x.Timestamp).ToList();
                            break;
                        case "createByUserName":
                            _activityLogViewModelList = _activityLogViewModelList.OrderByDescending(x => x.CreateByUserName).ToList();
                            break;
                        case "userRole":
                            _activityLogViewModelList = _activityLogViewModelList.OrderByDescending(x => x.UserRole).ToList();
                            break;
                        case "changeDetails":
                            _activityLogViewModelList = _activityLogViewModelList.OrderByDescending(x => x.ChangeDetails).ToList();
                            break;
                        case "activity":
                            _activityLogViewModelList = _activityLogViewModelList.OrderByDescending(x => x.Activity).ToList();
                            break;
                        case "id":
                            _activityLogViewModelList = _activityLogViewModelList.OrderByDescending(x => x.Id).ToList();
                            break;
                        default:
                            _activityLogViewModelList = _activityLogViewModelList.OrderByDescending(x => x.Id).ToList();
                            break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(dataTableParamsVM.searchValue))
            {
                _activityLogViewModelList = _activityLogViewModelList.Where(m => m.UserId.ToString().Contains(dataTableParamsVM.searchValue)
                                            || m.CreateByUserName.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.UserRole.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.ChangeDetails.ToLower().Contains(dataTableParamsVM.searchValue)).ToList();
            }

            return _activityLogViewModelList;
        }



        private string GetLoggedInUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId; // Returns the logged-in user's ID
        }

        private string GetLoggedInRole()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
            return userId; // Returns the logged-in user's ID
        }

    }


}
