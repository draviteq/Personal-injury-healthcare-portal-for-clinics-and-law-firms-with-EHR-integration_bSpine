using CapitalBraineAndSpine.Services.ViewModels.RolesAndPermissionModels;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.RolesAndServiceAbstracts
{
    public interface IRoleAndPermissionService
    {
        Task<List<Role>> GetRoles(string roleId = "");

        Task<List<Data.Entities.RoleAndPermissionEntities.Permission>> GetPermissions();

        Task<bool> UpdatePermissions(Role role);
    }
}
