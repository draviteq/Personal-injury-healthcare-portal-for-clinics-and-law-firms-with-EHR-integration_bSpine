using EntityFrameworkCore.EncryptColumn.Attribute;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapitalBraineAndSpine.Data.Entities.LogsEntities
{
    public class AuditLogs
    {
        public int Id { get; set; }


        [Column(TypeName = "nvarchar(500)")]
        public string UserId { get; set; }
        public string UserRole { get; set; }// New/Modified user's userId
        [EncryptColumn]
        [Column(TypeName = "nvarchar(500)")]
        public string PatientId { get; set; } // Patient Id which logged in user has access
        [EncryptColumn]
        [Column(TypeName = "nvarchar(500)")]
        public string Activity { get; set; } // Activity perform by the user Created/Modified/View
        [EncryptColumn]
        [Column(TypeName = "nvarchar(500)")]
        public string EntityName { get; set; } // Table Name
        [EncryptColumn]
        [Column(TypeName = "nvarchar(500)")]
        public string ChangeDetails { get; set; }
        public string IpAddress { get; set; } // User IP Address

        [Column(TypeName = "nvarchar(500)")]
        public string CreatedByUserId { get; set; } // Action perform by user Id
        public string CreatedByRole { get; set; } // Role of user who is preforming the action
        public DateTime Timestamp { get; set; } // Date and time of action
    }
}
