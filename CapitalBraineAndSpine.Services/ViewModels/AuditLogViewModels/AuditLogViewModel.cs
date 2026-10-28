namespace CapitalBraineAndSpine.Services.ViewModels.AuditLogViewModels
{
    public class AuditLogViewModel
    {
        public int ActivityId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime TimeStamp { get; set; }
        public string DisplayDate { get; set; }
    }

}
