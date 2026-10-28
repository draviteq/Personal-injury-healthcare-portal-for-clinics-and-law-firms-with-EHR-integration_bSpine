using HealthCare.Data.Entities.IdentityEntities;
using HealthCare.Services.Services.Abstracts.AuditLogAbstracts;
using HealthCare.Services.Services.Abstracts.CommonAbstracts;
using HealthCare.Services.ViewModels.DataDownloadRequestViewModels;
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
    [Authorize(Roles = "superadmin,lawfirm,admin,staff")]
    public class AuditLogController : CBSBaseController
    {
        private readonly UserManager<CBSUser> _userManager;
        private readonly IAuditLogService _activityLogService;
        private readonly ICommonService _commonService;
        public AuditLogController(UserManager<CBSUser> userManager,
            IAuditLogService activityLogService,
            ICommonService commonService) : base(userManager)
        {
            _userManager = userManager;
            _activityLogService = activityLogService;
            _commonService = commonService;
        }


        /// <summary>
        /// Activity Log List
        /// </summary>
        /// <returns></returns>
        [HttpGet("/[Controller]")]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Activity Log Deails
        /// </summary>
        /// <returns></returns>

        [HttpPost("list")]
        public async Task<IActionResult> AuditLogs()
        {
            CBSUser user = await GetCurrentUserAsync();
            DataTableParamsVM dataTableParamsVM = new DataTableParamsVM
            {
                userId = user.Id,
                userRole = _commonService.GetUserRoleByUserId(user.Id).Name,
                draw = Request.Form["draw"].FirstOrDefault(),
                start = Request.Form["start"].FirstOrDefault(),
                length = Request.Form["length"].FirstOrDefault(),
                sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault(),
                sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault(),
                searchValue = Request.Form["search[value]"].FirstOrDefault()?.ToLower(),
                pageSize = Request.Form["length"].FirstOrDefault() != null ? Convert.ToInt32(Request.Form["length"].FirstOrDefault()) : 0,
                skip = Request.Form["start"].FirstOrDefault() != null ? Convert.ToInt32(Request.Form["start"].FirstOrDefault()) : 0,
                extraParam = Convert.ToString(Request.Form["isinternal"].FirstOrDefault()),
                recordsTotal = 0,
            };
            var _dataDownloadRequestList = await _activityLogService.GetActivityLogsByUserRole(dataTableParamsVM);
            var jsonData = new { draw = dataTableParamsVM.draw, recordsFiltered = dataTableParamsVM.recordsTotal, recordsTotal = dataTableParamsVM.recordsTotal, data = _dataDownloadRequestList };
            return Ok(jsonData);
        }
    }
}
