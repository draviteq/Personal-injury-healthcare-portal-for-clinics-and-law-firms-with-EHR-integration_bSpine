using HealthCare.Data.Entities.IdentityEntities;
using HealthCare.Services.Services.Abstracts.CommonAbstracts;
using HealthCare.Services.Services.Abstracts.DataDownloadRequestAbstracts;
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
    public class DataRequestController : CBSBaseController
    {
        private readonly UserManager<CBSUser> _userManager;
        private readonly IDataDownlaodRequestService _dataDownlaodRequestService;
        private readonly ICommonService _commonService;
        public DataRequestController(UserManager<CBSUser> userManager, IDataDownlaodRequestService dataDownlaodRequestService, ICommonService commonService) : base(userManager)
        {
            _userManager = userManager;
            _dataDownlaodRequestService = dataDownlaodRequestService;
            _commonService = commonService;
        }

        /// <summary>
        /// Data Request List
        /// </summary>
        /// <returns></returns>
        [HttpGet("/[Controller]")]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Get Data Request Details
        /// </summary>
        /// <returns></returns>

        [HttpPost("getdatarequestdetails")]
        public async Task<IActionResult> GetDataRequestDetails()
        {
            CBSUser loggedInUser = await GetCurrentUserAsync();

            // check if userid available in form data
            var lawfirmUserId = Request.Form["lawfirmUserId"].FirstOrDefault();
            DataTableParamsVM dataTableParamsVM = new DataTableParamsVM
            {
                userId = loggedInUser.Id,
                userRole = _commonService.GetUserRoleByUserId(loggedInUser.Id).Name,
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
            var _dataDownloadRequestList = await _dataDownlaodRequestService.GetPateintDataRequestByUserIdAsync(dataTableParamsVM, lawfirmUserId);
            var jsonData = new { draw = dataTableParamsVM.draw, recordsFiltered = dataTableParamsVM.recordsTotal, recordsTotal = dataTableParamsVM.recordsTotal, data = _dataDownloadRequestList };
            return Ok(jsonData);
        }

        /// <summary>
        /// Accept request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        [HttpGet("acceptrequest")]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {

            PatientDataRequestViewModel patientDataRequestViewModel = await _dataDownlaodRequestService.AcceptRequestByRequestId(requestId, (await GetCurrentUserAsync()).Id);
            return Ok();
        }

        /// <summary>
        /// Reject request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="rejectReason"></param>
        /// <returns></returns>
        [HttpGet("rejectrequest")]
        public async Task<IActionResult> RejectRequest(int requestId, string rejectReason)
        {
            string userId = (await GetCurrentUserAsync()).Id;
            PatientDataRequestViewModel patientDataRequestViewModel = await _dataDownlaodRequestService.RejectRequestByRequestId(requestId, rejectReason, (await GetCurrentUserAsync()).Id);
            return Ok();

        }
    }
}
