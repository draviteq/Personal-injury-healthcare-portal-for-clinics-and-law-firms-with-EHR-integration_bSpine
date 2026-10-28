using CapitalBraineAndSpine.Services.ViewModels.AuditLogViewModels;
using CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.AuditLogAbstracts
{
    public interface IAuditLogService
    {
        Task<List<AuditLogViewItem>> GetActivityLogsByUserRole(DataTableParamsVM dataTableParamsVM);

        Task<bool> AuditLogsAsync(bool isNew = false, string createdUserId = "", string createdUserRole = "");
        Task<bool> AuditLogsAsync(string userId, string activity);
        bool AuditLogsAsync(string patientName, string patientId, string dob, string activity = "");

    }
}
