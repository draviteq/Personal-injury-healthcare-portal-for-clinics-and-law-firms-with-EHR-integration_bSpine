using HealthCare.Data.Entities.IdentityEntities;
using HealthCare.Services.Services.Abstracts.RolesAndServiceAbstracts;
using HealthCare.Services.ViewModels.RolesAndPermissionModels;
using HealthCare.Web.Infrastructures.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HealthCare.Web.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    [Authorize(Roles = "superadmin")]
    public class RolesPermissionController : CBSBaseController
    {
        private readonly UserManager<CBSUser> _userManager;
        private readonly IRoleAndPermissionService _roleAndPermissionService;
        public RolesPermissionController(IRoleAndPermissionService roleAndPermissionService, UserManager<CBSUser> userManager) : base(userManager)
        {
            _userManager = userManager;
            _roleAndPermissionService = roleAndPermissionService;


        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get General Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var model = await _roleAndPermissionService.GetRoles();
            return Ok(model);

        }
        /// <summary>
        /// Get General Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet("roles/{roleId}")]
        public async Task<IActionResult> GetRoles(string roleId)
        {
            var model = await _roleAndPermissionService.GetRoles(roleId);
            return Ok(model);

        }


        /// <summary>
        /// Get General Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet("permissions")]
        public async Task<IActionResult> GetPermissions()
        {
            var model = await _roleAndPermissionService.GetPermissions();
            return Ok(model);

        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateRole([FromBody] Role updatedRole)
        {

            var result = await _roleAndPermissionService.UpdatePermissions(updatedRole);
            if (!result)
                return Ok(GetAjaxResponse(true, "Unable to update permissions", result));

            return Ok(GetAjaxResponse(true, "Permissions updated successfully.", result));
        }


    }
}
