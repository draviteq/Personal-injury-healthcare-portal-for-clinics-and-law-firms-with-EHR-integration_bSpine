using AutoMapper;
using CapitalBraineAndSpine.Data.Contexts;
using CapitalBraineAndSpine.Services.Services.Abstracts.UserAbstracts;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace CapitalBraineAndSpine.Services.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly BaseDbContext _baseDbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        // private readonly IWebHostEnvironment _webHostEnvironment;

        public UserService(BaseDbContext baseDbContext, IMapper mapper,
            IConfiguration configuration)
        {
            _baseDbContext = baseDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        /// <summary>
        /// Get USA states list
        /// </summary>
        /// <returns></returns>
        public async Task<List<UsaStatesViewModel>> StateListAsync()
        {
            return await _baseDbContext.UsaStates.Select(x => new UsaStatesViewModel { Code = x.Code, Name = x.Name }).ToListAsync();

        }


    }
}
