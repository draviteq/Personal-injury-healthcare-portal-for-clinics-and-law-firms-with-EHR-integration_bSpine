using CapitalBraineAndSpine.Data.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapitalBraineAndSpine.Data.Entities.SettingEntities
{
    public class RegionalSettings : BaseEntity
    {
        public int Language { get; set; }
        public int Currency { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string TimeZone { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public double Daylight { get; set; }
    }
}
