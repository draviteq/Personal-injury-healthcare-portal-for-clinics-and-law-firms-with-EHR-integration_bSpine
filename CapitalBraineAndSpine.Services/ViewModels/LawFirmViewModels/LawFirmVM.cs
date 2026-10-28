namespace CapitalBraineAndSpine.Services.ViewModels.LawFirmViewModels
{
    public class LawFirmVM
    {
        public string Id { get; set; }
        public string CustomUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LawFirmName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int DownloadRequest { get; set; }
        public string DisplayDate { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
