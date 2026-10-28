using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalBraineAndSpine.Services.ViewModels.SettingViewModels
{
    public class ResponseViewModel
    {
        public bool Status {  get; set; }
        public string StatusMessage { get; set; }

        public ResponseViewModel(bool isSuccess, string message)
        {
            Status = isSuccess;
            StatusMessage = message;
        }
    }
}
