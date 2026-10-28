using CapitalBraineAndSpine.Data.Contexts;
using CapitalBraineAndSpine.Data.Entities.IdentityEntities;
using CapitalBraineAndSpine.Data.Entities.RoleAndPermissionEntities;
using CapitalBraineAndSpine.Service.Infrastructures.Helpers;
using CapitalBraineAndSpine.Services.Services.Abstracts.EmailAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.RolesAndServiceAbstracts;
using CapitalBraineAndSpine.Services.ViewModels.RolesAndPermissionModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CapitalBraineAndSpine.Services.Services.RolesAndPermissionServices
{
    public class RoleAndPermissionService : IRoleAndPermissionService
    {
        private readonly BaseDbContext _baseDbContext;
        private readonly IEmailService _emailService;
        private readonly UserManager<CBSUser> _userManager;
        private readonly RoleManager<CBSRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleAndPermissionService(BaseDbContext baseDbContext

            , UserManager<CBSUser> userManager
            , RoleManager<CBSRole> roleManager
            , IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _baseDbContext = baseDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get Roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<Role>> GetRoles(string roleId = "")
        {
            var roles = new List<Role>();

            if (!string.IsNullOrEmpty(roleId))
            {
                roles = await _baseDbContext.Roles.Where(x => x.Name != "superadmin" && x.Id == roleId).Select(x => new Role
                {
                    Name = x.DisplayName,
                    Id = x.Id
                }).ToListAsync();
            }
            else
            {
                roles = await _baseDbContext.Roles.Where(x => x.Name != "superadmin").Select(x => new Role
                {
                    Name = x.DisplayName,
                    Id = x.Id
                }).ToListAsync();
            }

            foreach (var role in roles)
            {
                role.Permissions = _baseDbContext.RolePermissions.Join(_baseDbContext.Permissions
                    , rp => rp.PermissionId
                    , p => p.Id
                    , (rp, p) => new { RP = rp, P = p }).Where(x => x.RP.RoleId == role.Id)
    .Select(x => x.P.Name.ToString())
    .ToList();
            }

            return roles;

        }

        /// <summary>
        /// Get Permissions
        /// </summary>
        /// <returns></returns>
        public async Task<List<Data.Entities.RoleAndPermissionEntities.Permission>> GetPermissions()
        {
            return await _baseDbContext.Permissions.Select(x => new Data.Entities.RoleAndPermissionEntities.Permission
            {
                Name = x.Name,
                Id = x.Id
            }).ToListAsync();

        }
        /// <summary>
        /// Update Permissions
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdatePermissions(Role role)
        {
            var existingPermission = await _baseDbContext.RolePermissions.Where(x => x.RoleId == role.Id).ToListAsync();
            _baseDbContext.RolePermissions.RemoveRange(existingPermission);
            _baseDbContext.SaveChanges();

            foreach (var rolePermission in role.Permissions)
            {
                // Add new permissions
                var permission = await _baseDbContext.Permissions.Where(x => x.Name.ToLower() == rolePermission.ToLower()).FirstOrDefaultAsync();


                RolePermission role_Permission = new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permission.Id,
                    CreatedDate = CBSHelpers.CurrentLocalDateTime(),
                    LastUpdated = CBSHelpers.CurrentLocalDateTime()
                };
                _baseDbContext.RolePermissions.Add(role_Permission);
                _baseDbContext.SaveChanges();
            }
            return true;


        }
    }
}
