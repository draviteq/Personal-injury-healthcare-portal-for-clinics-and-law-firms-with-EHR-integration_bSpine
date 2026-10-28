using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalBraineAndSpine.Services.ViewModels.LawFirmViewModels
{
    public class RolePermissionVM
    {
        public string RoleId { get; set; }
        public int PermissionId { get; set; }
        public string RoleName { get; set; }
        public string PermissionName { get; set; }
        public bool IsSelected { get; set; }
    }
    public class RolePermissionViewModel
    {
       public List<RolePermissionVM> RolePermissions { get; set; }
        public string TargetRole {  get; set; }
    }
}
