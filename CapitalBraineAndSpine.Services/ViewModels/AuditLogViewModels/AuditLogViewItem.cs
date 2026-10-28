using CapitalBraineAndSpine.Data.Entities.LogsEntities;

namespace CapitalBraineAndSpine.Services.ViewModels.AuditLogViewModels
{
    public class AuditLogViewItem : AuditLogs
    {
        public string CreateByUserName { get; set; }
        public string UserName { get; set; }
        public string DisplayDate { get; set; }

    }
}
