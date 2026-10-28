using CapitalBraineAndSpine.Services.ViewModels.ProfileViewModel;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalBraineAndSpine.Services.Services.Abstracts.UserAbstracts
{
    public interface IUserService
    {

      
        Task<List<UsaStatesViewModel>> StateListAsync();
       
    }
}
