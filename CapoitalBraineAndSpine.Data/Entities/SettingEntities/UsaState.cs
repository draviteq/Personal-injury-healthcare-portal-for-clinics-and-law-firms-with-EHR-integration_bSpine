using CapitalBraineAndSpine.Data.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapitalBraineAndSpine.Data.Entities.SettingEntities
{
    public class UsaStates : BaseEntity
    {
        [Column(TypeName = "nvarchar(4)")]
        public string Code { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }
    }
}
