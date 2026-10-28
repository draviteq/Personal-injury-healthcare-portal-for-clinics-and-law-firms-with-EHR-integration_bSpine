using CapitalBraineAndSpine.Data.Abstracts;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapitalBraineAndSpine.Data.Entities.SettingEntities
{
    public class EmailTemplates : BaseEntity
    {

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string? Name { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(max)")]
        public string ContentData { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

    }
}
