using CapitalBraineAndSpine.Data.Abstracts;

namespace CapitalBraineAndSpine.Data.Entities.RoleAndPermissionEntities
{
    public class RolePermission : BaseEntity
    {
        public int PermissionId { get; set; }
        public string RoleId { get; set; }
    }
}
