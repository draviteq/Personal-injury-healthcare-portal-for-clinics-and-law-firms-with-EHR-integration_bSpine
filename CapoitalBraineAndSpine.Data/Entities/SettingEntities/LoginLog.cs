using CapitalBraineAndSpine.Data.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CATT.Data.Entities.SettingEntities
{
    public class LoginLog : BaseEntity
    {
        [Column(TypeName = "nvarchar(100)")]
        public string? LastLoginIp { get; set; }
    }
}
