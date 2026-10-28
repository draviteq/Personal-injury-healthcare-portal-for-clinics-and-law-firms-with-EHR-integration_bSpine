using CapitalBraineAndSpine.Data.Abstracts;

namespace CapitalBraineAndSpine.Data.Entities.SettingEntities
{
    public class GeneralSetting : BaseEntity
    {
        public int SettingType { get; set; }
        public string SettingValue { get; set; }

    }
}
