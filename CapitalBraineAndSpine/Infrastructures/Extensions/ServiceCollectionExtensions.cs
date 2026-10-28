using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using HealthCare.Data.Contexts;

namespace HealthCare.Web.Infrastructures.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataStore(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;

            services.AddDbContextPool<BaseDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SQLConnection"),
                 b => b.MigrationsAssembly(migrationsAssembly)
                 .CommandTimeout(180)));

            return services;
            }
    }
}
