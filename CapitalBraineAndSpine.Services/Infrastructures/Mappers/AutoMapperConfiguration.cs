using AutoMapper;
using CapitalBraineAndSpine.Service.Infrastructures.Mappers.Profiles;

namespace CapitalBraineAndSpine.Service.Infrastructures.Mappers
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            var config = new MapperConfiguration(x =>
            {
                x.AddProfile<DomainToViewModel>();
                x.AddProfile<ViewModelToDomain>();
            });

            IMapper mapper = config.CreateMapper();
        }
    }
}
