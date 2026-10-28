using CapitalBraineAndSpine.Data.Abstracts;

namespace CapitalBraineAndSpine.Data.Entities.RoleAndPermissionEntities
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
