using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.EmailAbstracts
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailViewModel emailViewModel);
        string GetMailBody(string template, IDictionary vars = null);
    }
}
