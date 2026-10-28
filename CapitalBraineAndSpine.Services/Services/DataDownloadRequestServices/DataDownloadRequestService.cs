using CapitalBraineAndSpine.Service.Infrastructures.ExtensionHelper;
using CapitalBraineAndSpine.Service.Infrastructures.Helpers;
using CapitalBraineAndSpine.Services.Infrastructures.Enums;
using CapitalBraineAndSpine.Services.Infrastructures.Helpers;
using CapitalBraineAndSpine.Services.Services.Abstracts.AuditLogAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.DataDownloadRequestAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.NotificationAbstracts;
using CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels;
using CapitalBraineAndSpine.Services.ViewModels.NotificationViewModels;
using System.Data;

namespace CapitalBraineAndSpine.Services.Services.DataDownloadRequestServices
{
    public class DataDownloadRequestService : IDataDownlaodRequestService
    {
        private readonly BaseDbContext _baseDbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;
        private readonly IAuditLogService _activityLogService;

        public DataDownloadRequestService(BaseDbContext baseDbContext, IMapper mapper,
            IConfiguration configuration, IHttpContextAccessor httpContextAccessor, INotificationService notificationService
            , IAuditLogService auditLogService
            )
        {
            _baseDbContext = baseDbContext;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _activityLogService = auditLogService;

        }

        /// <summary>
        /// Accpet Patient Data Request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="loggedUserId"></param>
        /// <returns></returns>
        public async Task<PatientDataRequestViewModel> AcceptRequestByRequestId(int requestId, string loggedUserId)
        {
            var patientDataRequestViewModel = new PatientDataRequestViewModel();
            var requestDetails = await _baseDbContext.PatientDataRequest.Where(x => x.Id == requestId).SingleOrDefaultAsync();
            if (requestDetails != null)
            {


                requestDetails.Status = (Int32)CapitalBraineAndSpine.Services.Infrastructures.Enums.Status.Accepted;
                requestDetails.LastUpdated = CBSHelpers.CurrentLocalDateTime();
                _baseDbContext.SaveChanges();
                patientDataRequestViewModel.PatientId = Convert.ToString(requestDetails.PatientId);
                patientDataRequestViewModel.LawFirmName = await _baseDbContext.Users.Where(x => x.Id == requestDetails.LawFirmUserId).Select(x => x.LawFirmName).FirstOrDefaultAsync();
                patientDataRequestViewModel.Status = requestDetails.Status;
                patientDataRequestViewModel.LawFirmUserId = requestDetails.LawFirmUserId;

                // Get Patient Details
                var patientDetails = await _baseDbContext.Patient.Where(x => x.Id == requestDetails.PatientId).FirstOrDefaultAsync();

                // Create Audit log
                _activityLogService.AuditLogsAsync($"{patientDetails.FirstName} {patientDetails.LastName}", patientDetails.TebraPatientId, patientDetails.DateOfBirth, "Request Approved");

            }


            // Create Notification

            NotificationViewModel notification = new NotificationViewModel()
            {
                TriggerByUserId = loggedUserId,
                TriggerForUserId = requestDetails.CreatedBy,
                SourceType = (int)SourceType.Notification,
                Type = (int)NotificationType.RequestAccepted,
                PatientOrUserId = Convert.ToString(requestDetails.PatientId)
            };

            _notificationService.CreateNotification(notification, loggedUserId);
            return patientDataRequestViewModel;


        }

        /// <summary>
        /// Get Patient Data Request By UserId
        /// </summary>
        /// <param name="dataTableParamsVM"></param>
        /// <param name="lawfirmUserId"></param>
        /// <returns></returns>
        public async Task<List<PatientDataRequestViewModel>> GetPateintDataRequestByUserIdAsync(DataTableParamsVM dataTableParamsVM, string lawfirmUserId = "")
        {
            // Get logged-In user
            var user = _httpContextAccessor.HttpContext.User;


            var _dataDownloadRequestList = new List<PatientDataRequestViewModel>();


            // Check dataTable pageSize
            if (dataTableParamsVM.pageSize > 0)
            {
                if (string.IsNullOrEmpty(lawfirmUserId))
                {
                    // Check role
                    if (RoleHelper.UserHasAnyRole(user, Roles.SuperAdmin, Roles.CBSAdmin, Roles.CBSStaff))
                    {
                        // Check if LawFirmUserId is available

                        _dataDownloadRequestList = await _baseDbContext.PatientDataRequest
                            .Join(
                            _baseDbContext.Patient,
                            pd => Convert.ToString(pd.PatientId),
                            p => Convert.ToString(p.Id),
                            (pd, p) => new { PD = pd, P = p })
                            .OrderByDescending(x => x.PD.CreatedDate)
                            .Skip(dataTableParamsVM.skip)
                            .Take(dataTableParamsVM.pageSize)
                            .Select(x => new PatientDataRequestViewModel
                            {
                                Id = x.PD.Id,
                                LawFirmUserId = x.PD.LawFirmUserId,
                                LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName != null ? _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName : string.Empty,
                                PatientId = Encryptions.Encrypt(Convert.ToString(x.PD.PatientId), ConfigurationHelper.KeyForEncryption(_configuration)),
                                Status = x.PD.Status,
                                Reason = x.PD.Reason,
                                LastUpdated = x.PD.LastUpdated.ToDateWithDDMMYYYY(),
                                CreatedDate = x.PD.CreatedDate.ToDateWithDDMMYYYY(),
                                PatientName = $"{x.P.FirstName} {x.P.LastName}",
                                DateOfBirth = x.P.DateOfBirth,
                                userRole = dataTableParamsVM.userRole,
                                TebraPatientId = Encryptions.Encrypt(x.P.TebraPatientId, ConfigurationHelper.KeyForEncryption(_configuration)),
                                LastUpdatedFormatted = x.PD.LastUpdated,
                                CreatedDateFormatted = x.PD.CreatedDate,
                                UserId = x.PD.UserId

                            }).ToListAsync();


                    }
                    else if (RoleHelper.UserHasAnyRole(user, Roles.LawFirm))
                    {

                        var patientDataRequestQuery = from pd in _baseDbContext.PatientDataRequest
                                                      join p in _baseDbContext.Patient
                                                          on Convert.ToString(pd.PatientId) equals Convert.ToString(p.Id)
                                                      join u in _baseDbContext.Users
                                                          on pd.LawFirmUserId equals u.Id into userGroup
                                                      from usr in userGroup.DefaultIfEmpty()
                                                      join pay in _baseDbContext.Payments
                                                          on pd.Id equals pay.PatientDataRequestId into paymentGroup
                                                      from payment in paymentGroup.DefaultIfEmpty()
                                                      orderby pd.CreatedDate descending
                                                      select new PatientDataRequestViewModel
                                                      {
                                                          Id = pd.Id,
                                                          LawFirmUserId = pd.LawFirmUserId,
                                                          LawFirmName = usr != null ? usr.LawFirmName : string.Empty,
                                                          PatientId = Encryptions.Encrypt(Convert.ToString(pd.PatientId), ConfigurationHelper.KeyForEncryption(_configuration)),
                                                          Status = pd.Status,
                                                          Reason = pd.Reason,
                                                          LastUpdated = pd.LastUpdated.ToDateWithDDMMYYYY(),
                                                          CreatedDate = pd.CreatedDate.ToDateWithDDMMYYYY(),
                                                          PatientName = p != null ? $"{p.FirstName} {p.LastName}" : string.Empty,
                                                          DateOfBirth = p.DateOfBirth, // Nullable handling
                                                          userRole = dataTableParamsVM.userRole,
                                                          TebraPatientId = p != null ? Encryptions.Encrypt(p.TebraPatientId, ConfigurationHelper.KeyForEncryption(_configuration)) : string.Empty,
                                                          LastUpdatedFormatted = pd.LastUpdated,
                                                          CreatedDateFormatted = pd.CreatedDate,
                                                          UserId = pd.UserId,
                                                          PaymentStatusId = payment != null ? payment.Status.ToString() : string.Empty
                                                      };

                        _dataDownloadRequestList = await patientDataRequestQuery.ToListAsync();
                    }
                    else if (RoleHelper.UserHasAnyRole(user, Roles.Attorney))
                    {
                        _dataDownloadRequestList = await _baseDbContext.PatientDataRequest
                            .Where(x => x.UserId == dataTableParamsVM.userId)
                            .Join(
                                _baseDbContext.Patient,
                                pd => Convert.ToString(pd.PatientId),
                                p => Convert.ToString(p.Id),
                                (pd, p) => new { PD = pd, P = p }).Where(x => x.PD.UserId == dataTableParamsVM.userId)
                                .OrderByDescending(x => x.PD.CreatedDate)
                                .Skip(dataTableParamsVM.skip).Take(dataTableParamsVM.pageSize)
                                .Select(x => new PatientDataRequestViewModel
                                {
                                    Id = x.PD.Id,
                                    LawFirmUserId = x.PD.LawFirmUserId,
                                    LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName != null ? _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName : string.Empty,
                                    PatientId = Encryptions.Encrypt(Convert.ToString(x.PD.PatientId), ConfigurationHelper.KeyForEncryption(_configuration)),
                                    Status = x.PD.Status,
                                    Reason = x.PD.Reason,
                                    LastUpdated = x.PD.LastUpdated.ToDateWithDDMMYYYY(),
                                    CreatedDate = x.PD.CreatedDate.ToDateWithDDMMYYYY(),
                                    PatientName = $"{x.P.FirstName} {x.P.LastName}",
                                    DateOfBirth = x.P.DateOfBirth,
                                    userRole = dataTableParamsVM.userRole,
                                    TebraPatientId = Encryptions.Encrypt(x.P.TebraPatientId, ConfigurationHelper.KeyForEncryption(_configuration)),
                                    LastUpdatedFormatted = x.PD.LastUpdated,
                                    CreatedDateFormatted = x.PD.CreatedDate,
                                    UserId = x.PD.UserId

                                }).ToListAsync();
                    }
                }
                else
                {
                    if (RoleHelper.UserHasAnyRole(user, Roles.SuperAdmin, Roles.CBSAdmin, Roles.CBSStaff))
                    {
                        // Check if LawFirmUserId is available


                        _dataDownloadRequestList = await _baseDbContext.PatientDataRequest.Where(x => x.LawFirmUserId == lawfirmUserId)
                            .Join(
                            _baseDbContext.Patient,
                            pd => Convert.ToString(pd.PatientId),
                            p => Convert.ToString(p.Id),
                            (pd, p) => new { PD = pd, P = p })
                            .OrderByDescending(x => x.PD.CreatedDate)
                            .Skip(dataTableParamsVM.skip)
                            .Take(dataTableParamsVM.pageSize)
                            .Select(x => new PatientDataRequestViewModel
                            {
                                Id = x.PD.Id,
                                LawFirmUserId = x.PD.LawFirmUserId,
                                LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName != null ? _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName : string.Empty,
                                PatientId = Encryptions.Encrypt(Convert.ToString(x.PD.PatientId), ConfigurationHelper.KeyForEncryption(_configuration)),
                                Status = x.PD.Status,
                                Reason = x.PD.Reason,
                                LastUpdated = x.PD.LastUpdated.ToDateWithDDMMYYYY(),
                                CreatedDate = x.PD.CreatedDate.ToDateWithDDMMYYYY(),
                                PatientName = $"{x.P.FirstName} {x.P.LastName}",
                                DateOfBirth = x.P.DateOfBirth,
                                userRole = dataTableParamsVM.userRole,
                                TebraPatientId = Encryptions.Encrypt(x.P.TebraPatientId, ConfigurationHelper.KeyForEncryption(_configuration)),
                                LastUpdatedFormatted = x.PD.LastUpdated,
                                CreatedDateFormatted = x.PD.CreatedDate,
                                UserId = x.PD.UserId

                            }).ToListAsync();


                    }
                    else if (RoleHelper.UserHasAnyRole(user, Roles.LawFirm))
                    {
                        _dataDownloadRequestList = await _baseDbContext.PatientDataRequest
                            .Where(x => x.LawFirmUserId == lawfirmUserId)
                            .Join(
                                _baseDbContext.Patient,
                                pd => Convert.ToString(pd.PatientId),
                                p => Convert.ToString(p.Id),
                                (pd, p) => new { PD = pd, P = p }).Where(x => x.PD.LawFirmUserId == dataTableParamsVM.userId)
                                .OrderByDescending(x => x.PD.CreatedDate)
                                .Skip(dataTableParamsVM.skip).Take(dataTableParamsVM.pageSize)
                                .Select(x => new PatientDataRequestViewModel
                                {
                                    Id = x.PD.Id,
                                    LawFirmUserId = x.PD.LawFirmUserId,
                                    //LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName,
                                    LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName != null ? _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName : string.Empty,
                                    PatientId = Encryptions.Encrypt(Convert.ToString(x.PD.PatientId), ConfigurationHelper.KeyForEncryption(_configuration)),
                                    Status = x.PD.Status,
                                    Reason = x.PD.Reason,
                                    LastUpdated = x.PD.LastUpdated.ToDateWithDDMMYYYY(),
                                    CreatedDate = x.PD.CreatedDate.ToDateWithDDMMYYYY(),
                                    PatientName = $"{x.P.FirstName} {x.P.LastName}",
                                    DateOfBirth = x.P.DateOfBirth,
                                    userRole = dataTableParamsVM.userRole,
                                    TebraPatientId = Encryptions.Encrypt(x.P.TebraPatientId, ConfigurationHelper.KeyForEncryption(_configuration)),
                                    LastUpdatedFormatted = x.PD.LastUpdated,
                                    CreatedDateFormatted = x.PD.CreatedDate,
                                    UserId = x.PD.UserId

                                }).ToListAsync();
                    }
                    else if (RoleHelper.UserHasAnyRole(user, Roles.Attorney))
                    {
                        _dataDownloadRequestList = await _baseDbContext.PatientDataRequest
                            .Where(x => x.LawFirmUserId == lawfirmUserId)
                            .Join(
                                _baseDbContext.Patient,
                                pd => Convert.ToString(pd.PatientId),
                                p => Convert.ToString(p.Id),
                                (pd, p) => new { PD = pd, P = p }).Where(x => x.PD.UserId == dataTableParamsVM.userId)
                                .OrderByDescending(x => x.PD.CreatedDate)
                                .Skip(dataTableParamsVM.skip).Take(dataTableParamsVM.pageSize)
                                .Select(x => new PatientDataRequestViewModel
                                {
                                    Id = x.PD.Id,
                                    LawFirmUserId = x.PD.LawFirmUserId,
                                    //LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName,
                                    LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName != null ? _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName : string.Empty,
                                    PatientId = Encryptions.Encrypt(Convert.ToString(x.PD.PatientId), ConfigurationHelper.KeyForEncryption(_configuration)),
                                    Status = x.PD.Status,
                                    Reason = x.PD.Reason,
                                    LastUpdated = x.PD.LastUpdated.ToDateWithDDMMYYYY(),
                                    CreatedDate = x.PD.CreatedDate.ToDateWithDDMMYYYY(),
                                    PatientName = $"{x.P.FirstName} {x.P.LastName}",
                                    DateOfBirth = x.P.DateOfBirth,
                                    userRole = dataTableParamsVM.userRole,
                                    TebraPatientId = Encryptions.Encrypt(x.P.TebraPatientId, ConfigurationHelper.KeyForEncryption(_configuration)),
                                    LastUpdatedFormatted = x.PD.LastUpdated,
                                    CreatedDateFormatted = x.PD.CreatedDate,
                                    UserId = x.PD.UserId

                                }).ToListAsync();
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(lawfirmUserId))
                {
                    if (RoleHelper.UserHasAnyRole(user, Roles.SuperAdmin, Roles.CBSAdmin, Roles.CBSStaff))
                    {

                        // Check if LawFirmUserId is available

                        _dataDownloadRequestList = await _baseDbContext.PatientDataRequest.Join(
                            _baseDbContext.Patient,
                            pd => Convert.ToString(pd.PatientId),
                            p => Convert.ToString(p.Id),
                            (pd, p) => new { PD = pd, P = p })
                            .OrderByDescending(x => x.PD.CreatedDate)
                            .Select(x => new PatientDataRequestViewModel
                            {
                                Id = x.PD.Id,
                                LawFirmUserId = x.PD.LawFirmUserId,
                                LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName != null ? _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName : string.Empty,
                                PatientId = Encryptions.Encrypt(Convert.ToString(x.PD.PatientId), ConfigurationHelper.KeyForEncryption(_configuration)),
                                Status = x.PD.Status,
                                Reason = x.PD.Reason,
                                LastUpdated = x.PD.LastUpdated.ToDateWithDDMMYYYY(),
                                CreatedDate = x.PD.CreatedDate.ToDateWithDDMMYYYY(),
                                PatientName = $"{x.P.FirstName} {x.P.LastName}",
                                DateOfBirth = x.P.DateOfBirth,
                                userRole = dataTableParamsVM.userRole,
                                TebraPatientId = Encryptions.Encrypt(x.P.TebraPatientId, ConfigurationHelper.KeyForEncryption(_configuration)),
                                LastUpdatedFormatted = x.PD.LastUpdated,
                                CreatedDateFormatted = x.PD.CreatedDate,
                                UserId = x.PD.UserId

                            }).ToListAsync();

                    }
                    else if (RoleHelper.UserHasAnyRole(user, Roles.LawFirm, Roles.Attorney))
                    {
                        if (RoleHelper.UserHasAnyRole(user, Roles.Attorney))
                        {
                            dataTableParamsVM.userId = _baseDbContext.Users.Where(x => x.Id == dataTableParamsVM.userId).FirstOrDefault().ParentUserId;
                        }
                        _dataDownloadRequestList = await _baseDbContext.PatientDataRequest
                            .Where(x => x.LawFirmUserId == dataTableParamsVM.userId)
                            .Join(
                                _baseDbContext.Patient,
                                pd => Convert.ToString(pd.PatientId),
                                p => Convert.ToString(p.Id),
                                (pd, p) => new { PD = pd, P = p }).Where(x => x.PD.UserId == dataTableParamsVM.userId)
                                .OrderByDescending(x => x.PD.CreatedDate)
                                .Select(x => new PatientDataRequestViewModel
                                {
                                    Id = x.PD.Id,
                                    LawFirmUserId = x.PD.LawFirmUserId,
                                    LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName != null ? _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName : string.Empty,
                                    PatientId = Encryptions.Encrypt(Convert.ToString(x.PD.PatientId), ConfigurationHelper.KeyForEncryption(_configuration)),
                                    Status = x.PD.Status,
                                    Reason = x.PD.Reason,
                                    LastUpdated = x.PD.LastUpdated.ToDateWithDDMMYYYY(),
                                    CreatedDate = x.PD.CreatedDate.ToDateWithDDMMYYYY(),
                                    PatientName = $"{x.P.FirstName} {x.P.LastName}",
                                    DateOfBirth = x.P.DateOfBirth,
                                    userRole = dataTableParamsVM.userRole,
                                    TebraPatientId = Encryptions.Encrypt(x.P.TebraPatientId, ConfigurationHelper.KeyForEncryption(_configuration)),
                                    LastUpdatedFormatted = x.PD.LastUpdated,
                                    CreatedDateFormatted = x.PD.CreatedDate,
                                    UserId = x.PD.UserId

                                }).ToListAsync();
                    }
                }
                else
                {
                    if (RoleHelper.UserHasAnyRole(user, Roles.SuperAdmin, Roles.CBSAdmin, Roles.CBSStaff))
                    {
                        // Check if LawFirmUserId is available


                        _dataDownloadRequestList = await _baseDbContext.PatientDataRequest.Where(x => x.LawFirmUserId == lawfirmUserId)
                            .Join(
                            _baseDbContext.Patient,
                            pd => Convert.ToString(pd.PatientId),
                            p => Convert.ToString(p.Id),
                            (pd, p) => new { PD = pd, P = p })
                            .OrderByDescending(x => x.PD.CreatedDate)
                            .Skip(dataTableParamsVM.skip)
                            .Take(dataTableParamsVM.pageSize)
                            .Select(x => new PatientDataRequestViewModel
                            {
                                Id = x.PD.Id,
                                LawFirmUserId = x.PD.LawFirmUserId,
                                LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName != null ? _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName : string.Empty,
                                PatientId = Encryptions.Encrypt(Convert.ToString(x.PD.PatientId), ConfigurationHelper.KeyForEncryption(_configuration)),
                                Status = x.PD.Status,
                                Reason = x.PD.Reason,
                                LastUpdated = x.PD.LastUpdated.ToDateWithDDMMYYYY(),
                                CreatedDate = x.PD.CreatedDate.ToDateWithDDMMYYYY(),
                                PatientName = $"{x.P.FirstName} {x.P.LastName}",
                                DateOfBirth = x.P.DateOfBirth,
                                userRole = dataTableParamsVM.userRole,
                                TebraPatientId = Encryptions.Encrypt(x.P.TebraPatientId, ConfigurationHelper.KeyForEncryption(_configuration)),
                                LastUpdatedFormatted = x.PD.LastUpdated,
                                CreatedDateFormatted = x.PD.CreatedDate,
                                UserId = x.PD.UserId

                            }).ToListAsync();


                    }
                    else if (RoleHelper.UserHasAnyRole(user, Roles.LawFirm))
                    {
                        _dataDownloadRequestList = await _baseDbContext.PatientDataRequest.Where(x => x.LawFirmUserId == lawfirmUserId)
                            .Where(x => x.LawFirmUserId == dataTableParamsVM.userId)
                            .Join(
                                _baseDbContext.Patient,
                                pd => Convert.ToString(pd.PatientId),
                                p => Convert.ToString(p.Id),
                                (pd, p) => new { PD = pd, P = p }).Where(x => x.PD.LawFirmUserId == dataTableParamsVM.userId)
                                .OrderByDescending(x => x.PD.CreatedDate)
                                .Skip(dataTableParamsVM.skip).Take(dataTableParamsVM.pageSize)
                                .Select(x => new PatientDataRequestViewModel
                                {
                                    Id = x.PD.Id,
                                    LawFirmUserId = x.PD.LawFirmUserId,
                                    LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName != null ? _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName : string.Empty,
                                    PatientId = Encryptions.Encrypt(Convert.ToString(x.PD.PatientId), ConfigurationHelper.KeyForEncryption(_configuration)),
                                    Status = x.PD.Status,
                                    Reason = x.PD.Reason,
                                    LastUpdated = x.PD.LastUpdated.ToDateWithDDMMYYYY(),
                                    CreatedDate = x.PD.CreatedDate.ToDateWithDDMMYYYY(),
                                    PatientName = $"{x.P.FirstName} {x.P.LastName}",
                                    DateOfBirth = x.P.DateOfBirth,
                                    userRole = dataTableParamsVM.userRole,
                                    TebraPatientId = Encryptions.Encrypt(x.P.TebraPatientId, ConfigurationHelper.KeyForEncryption(_configuration)),
                                    LastUpdatedFormatted = x.PD.LastUpdated,
                                    CreatedDateFormatted = x.PD.CreatedDate,
                                    UserId = x.PD.UserId

                                }).ToListAsync();
                    }
                    else if (RoleHelper.UserHasAnyRole(user, Roles.Attorney))
                    {
                        _dataDownloadRequestList = await _baseDbContext.PatientDataRequest.Where(x => x.LawFirmUserId == lawfirmUserId)
                            .Where(x => x.LawFirmUserId == dataTableParamsVM.userId)
                            .Join(
                                _baseDbContext.Patient,
                                pd => Convert.ToString(pd.PatientId),
                                p => Convert.ToString(p.Id),
                                (pd, p) => new { PD = pd, P = p }).Where(x => x.PD.UserId == dataTableParamsVM.userId)
                                .OrderByDescending(x => x.PD.CreatedDate)
                                .Skip(dataTableParamsVM.skip).Take(dataTableParamsVM.pageSize)
                                .Select(x => new PatientDataRequestViewModel
                                {
                                    Id = x.PD.Id,
                                    LawFirmUserId = x.PD.LawFirmUserId,
                                    LawFirmName = _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName != null ? _baseDbContext.Users.Where(y => y.Id == x.PD.LawFirmUserId).FirstOrDefault().LawFirmName : string.Empty,
                                    PatientId = Encryptions.Encrypt(Convert.ToString(x.PD.PatientId), ConfigurationHelper.KeyForEncryption(_configuration)),
                                    Status = x.PD.Status,
                                    Reason = x.PD.Reason,
                                    LastUpdated = x.PD.LastUpdated.ToDateWithDDMMYYYY(),
                                    CreatedDate = x.PD.CreatedDate.ToDateWithDDMMYYYY(),
                                    PatientName = $"{x.P.FirstName} {x.P.LastName}",
                                    DateOfBirth = x.P.DateOfBirth,
                                    userRole = dataTableParamsVM.userRole,
                                    TebraPatientId = Encryptions.Encrypt(x.P.TebraPatientId, ConfigurationHelper.KeyForEncryption(_configuration)),
                                    LastUpdatedFormatted = x.PD.LastUpdated,
                                    CreatedDateFormatted = x.PD.CreatedDate,
                                    UserId = x.PD.UserId

                                }).ToListAsync();
                    }
                }
            }
            if (!(string.IsNullOrEmpty(dataTableParamsVM.sortColumn) && string.IsNullOrEmpty(dataTableParamsVM.sortColumnDirection)))
            {
                if (dataTableParamsVM.sortColumnDirection == SortDirection.Asc)
                {
                    switch (dataTableParamsVM.sortColumn)
                    {
                        case "id":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderBy(x => x.Id).ToList();
                            break;
                        case "statusName":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderBy(x => x.StatusName).ToList();
                            break;
                        case "lawFirmName":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderBy(x => x.LawFirmName).ToList();
                            break;
                        case "patientName":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderBy(x => x.PatientName).ToList();
                            break;
                        case "createdDate":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderBy(x => x.CreatedDateFormatted).ToList();
                            break;
                        case "lastUpdated":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderBy(x => x.LastUpdatedFormatted).ToList();
                            break;
                        default:
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderBy(x => x.CreatedDateFormatted).ToList();
                            break;
                    }
                }
                else
                {
                    switch (dataTableParamsVM.sortColumn)
                    {
                        case "id":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderByDescending(x => x.Id).ToList();
                            break;
                        case "statusName":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderByDescending(x => x.StatusName).ToList();
                            break;
                        case "lawFirmName":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderByDescending(x => x.LawFirmName).ToList();
                            break;
                        case "patientName":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderByDescending(x => x.PatientName).ToList();
                            break;
                        case "createdDate":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderByDescending(x => x.CreatedDateFormatted).ToList();
                            break;
                        case "lastUpdated":
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderByDescending(x => x.LastUpdatedFormatted).ToList();
                            break;
                        default:
                            _dataDownloadRequestList = _dataDownloadRequestList.OrderByDescending(x => x.CreatedDateFormatted).ToList();
                            break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(dataTableParamsVM.searchValue))
            {
                _dataDownloadRequestList = _dataDownloadRequestList.Where(m => m.Id.ToString().Contains(dataTableParamsVM.searchValue)
                                            || m.LawFirmName.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.StatusName.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.PatientName.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.LastUpdated.ToLower().Contains(dataTableParamsVM.searchValue)
                                            || m.CreatedDate.ToLower().Contains(dataTableParamsVM.searchValue)).ToList();
            }
            if (string.IsNullOrEmpty(lawfirmUserId))
            {
                if (RoleHelper.UserHasAnyRole(user, Roles.SuperAdmin))
                {
                    dataTableParamsVM.recordsTotal = _baseDbContext.PatientDataRequest.Join(
                            _baseDbContext.Patient,
                            pd => Convert.ToString(pd.PatientId),
                            p => Convert.ToString(p.Id),
                            (pd, p) => new { PD = pd, P = p }).Select(x => new PatientDataRequestViewModel
                            {
                                Id = x.PD.Id
                            }).Count();
                }
                else
                {
                    dataTableParamsVM.recordsTotal = _baseDbContext.PatientDataRequest.Join(
                            _baseDbContext.Patient,
                            pd => Convert.ToString(pd.PatientId),
                            p => Convert.ToString(p.Id),
                            (pd, p) => new { PD = pd, P = p }).Where(x => x.PD.LawFirmUserId == dataTableParamsVM.userId).Select(x => new PatientDataRequestViewModel
                            {
                                Id = x.PD.Id
                            }).Count();


                }
            }
            else
            {
                if (RoleHelper.UserHasAnyRole(user, Roles.SuperAdmin))
                {
                    dataTableParamsVM.recordsTotal = _baseDbContext.PatientDataRequest.Where(x => x.LawFirmUserId == lawfirmUserId).Join(
                            _baseDbContext.Patient,
                            pd => Convert.ToString(pd.PatientId),
                            p => Convert.ToString(p.Id),
                            (pd, p) => new { PD = pd, P = p }).Select(x => new PatientDataRequestViewModel
                            {
                                Id = x.PD.Id
                            }).Count();
                }
                else
                {
                    dataTableParamsVM.recordsTotal = _baseDbContext.PatientDataRequest.Where(x => x.LawFirmUserId == lawfirmUserId).Join(
                            _baseDbContext.Patient,
                            pd => Convert.ToString(pd.PatientId),
                            p => Convert.ToString(p.Id),
                            (pd, p) => new { PD = pd, P = p }).Where(x => x.PD.LawFirmUserId == dataTableParamsVM.userId).Select(x => new PatientDataRequestViewModel
                            {
                                Id = x.PD.Id
                            }).Count();


                }
            }

            return _dataDownloadRequestList;
        }

        /// <summary>
        /// Rejest request by Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="rejectReason"></param>
        /// <param name="loggedInUserId"></param>
        /// <returns></returns>
        public async Task<PatientDataRequestViewModel> RejectRequestByRequestId(int requestId, string rejectReason, string loggedInUserId)
        {
            var patientDataRequestViewModel = new PatientDataRequestViewModel();

            // Get Patient data request details
            var requestDetails = await _baseDbContext.PatientDataRequest.Where(x => x.Id == requestId).SingleOrDefaultAsync();


            // Check if request is available
            if (requestDetails != null)
            {
                requestDetails.Status = (Int32)CapitalBraineAndSpine.Services.Infrastructures.Enums.Status.Rejected;
                requestDetails.Reason = rejectReason;
                requestDetails.LastUpdated = CBSHelpers.CurrentLocalDateTime();
                _baseDbContext.SaveChanges();
                patientDataRequestViewModel.PatientId = Convert.ToString(requestDetails.PatientId);
                patientDataRequestViewModel.LawFirmName = await _baseDbContext.Users.Where(x => x.Id == requestDetails.LawFirmUserId).Select(x => x.LawFirmName).FirstOrDefaultAsync();
                patientDataRequestViewModel.Status = requestDetails.Status;
                patientDataRequestViewModel.LawFirmUserId = requestDetails.LawFirmUserId;

                // Get Patient Details
                var patientDetails = await _baseDbContext.Patient.Where(x => x.Id == requestDetails.PatientId).FirstOrDefaultAsync();

                // Create Audit log
                _activityLogService.AuditLogsAsync($"{patientDetails.FirstName} {patientDetails.LastName}", patientDetails.TebraPatientId, patientDetails.DateOfBirth, "Request Rejected");
            }



            // Create Notification

            NotificationViewModel notification = new NotificationViewModel()
            {
                TriggerByUserId = loggedInUserId,
                TriggerForUserId = requestDetails.CreatedBy,
                SourceType = (int)SourceType.Notification,
                Type = (int)NotificationType.RequestRejected,
                PatientOrUserId = Convert.ToString(requestDetails.PatientId)
            };

            _notificationService.CreateNotification(notification, loggedInUserId);

            return patientDataRequestViewModel;
        }
    }
}
