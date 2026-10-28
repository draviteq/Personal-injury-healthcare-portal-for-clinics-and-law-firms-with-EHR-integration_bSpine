using CapitalBraineAndSpine.Services.Services.Abstracts.AuditLogAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.CommonAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.DataDownloadRequestAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.EmailAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.LawFirmAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.NotificationAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.RolesAndServiceAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.SettingsAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.StripeAbstracts;
using CapitalBraineAndSpine.Services.Services.Abstracts.UserAbstracts;
using CapitalBraineAndSpine.Services.Services.ActivityLogServices;
using CapitalBraineAndSpine.Services.Services.CommonServices;
using CapitalBraineAndSpine.Services.Services.DataDownloadRequestServices;
using CapitalBraineAndSpine.Services.Services.EmailServices;
using CapitalBraineAndSpine.Services.Services.LawFirmServices;
using CapitalBraineAndSpine.Services.Services.RolesAndPermissionServices;
using CapitalBraineAndSpine.Services.Services.SettingsServices;
using CapitalBraineAndSpine.Services.Services.StripeServices;
using CapitalBraineAndSpine.Services.Services.UserServices;


namespace CapitalBraineAndSpine.Services.Services.Infrastructures.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            #region Application Services
            services.AddHttpContextAccessor();
            services.AddScoped(typeof(BaseDbContext));

            #endregion

            #region Serviecs
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDataDownlaodRequestService, DataDownloadRequestService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ILawFirmService, LawFirmService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IRoleAndPermissionService, RoleAndPermissionService>();
            services.AddScoped<IStripeService, StripeService>();

            #endregion

            return services;
        }
    }
}
