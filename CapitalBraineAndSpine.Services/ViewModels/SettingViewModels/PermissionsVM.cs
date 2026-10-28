using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalBraineAndSpine.Services.ViewModels.SettingViewModels
{
    public class PermissionsVM
    {
        public bool LawFirmManagement {  get; set; }
        public bool DataDownloadRequest { get; set; }
        public bool PatientManagement { get; set; }
        public bool StaffManagement { get; set; }
        public bool ActivityLogs { get; set; }

    }
}
