using CapitalBraineAndSpine.Data.Abstracts;

namespace CapitalBraineAndSpine.Data.Entities.SettingEntities
{
    public class CmsManagement : BaseEntity
    {
        public string PageName { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }
    }
}
