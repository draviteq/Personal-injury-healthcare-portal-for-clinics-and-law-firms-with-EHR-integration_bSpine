using EntityFrameworkCore.EncryptColumn.Attribute;

namespace CapitalBraineAndSpine.Services.ViewModels.LawFirmViewModels
{
    public class LawFirmStaffVM
    {
        public string Id { get; set; }
        public string CustomUserId { get; set; }
        [EncryptColumn]
        public string FirstName { get; set; }
        [EncryptColumn]
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string ParentUserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedOn { get; set; }
        //public string DisplayDate { get; set; }

    }
}
