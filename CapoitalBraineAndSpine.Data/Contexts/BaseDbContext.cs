using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using CapitalBraineAndSpine.Data.Entities;
using CapitalBraineAndSpine.Data.Entities.IdentityEntities;
using CapitalBraineAndSpine.Data.Entities.LogsEntities;
using CapitalBraineAndSpine.Data.Entities.NewFolder;
using CapitalBraineAndSpine.Data.Entities.ParientEntities;
using CapitalBraineAndSpine.Data.Entities.PaymentEntities;
using CapitalBraineAndSpine.Data.Entities.RoleAndPermissionEntities;
using CapitalBraineAndSpine.Data.Entities.SettingEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CapitalBraineAndSpine.Data.Contexts
{
    public class BaseDbContext : IdentityDbContext<CBSUser, CBSRole, string>
    {
        private readonly IEncryptionProvider _encryptionProvider = null;
        private readonly string _encryptionKey = "4o5pKX1ZV/Y6qIWIW/6YfQ==";



        public BaseDbContext()
        {
            _encryptionProvider = new GenerateEncryptionProvider(_encryptionKey);
        }

        public BaseDbContext(DbContextOptions options) : base(options)
        {

            _encryptionProvider = new GenerateEncryptionProvider(_encryptionKey);

        }

        public DbSet<UsaStates> UsaStates { get; set; }
        public DbSet<PatientDataRequest> PatientDataRequest { get; set; }
        public DbSet<Patient> Patient { get; set; }
        public DbSet<GeneralSetting> GeneralSettings { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<CmsManagement> CmsManagement { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<AuditLogs> AuditLogs { get; set; }
        public DbSet<StripeCustomer> StripeCustomers { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.UseEncryption(_encryptionProvider);
            base.OnModelCreating(builder);
        }

    }
}
