using EntityFrameworkCore.EncryptColumn.Attribute;
using CapitalBraineAndSpine.Services.ViewModels.DashboardViewModels;
using CapitalBraineAndSpine.Services.ViewModels.LawFirmViewModels;
using CapitalBraineAndSpine.Services.ViewModels.SettingViewModels;
using System.ComponentModel.DataAnnotations;

namespace CapitalBraineAndSpine.Services.ViewModels.AccountViewModels
{
    public class RegisterViewModel
    {
        public string Id { get; set; }

        [MaxLength(30)]
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        [EncryptColumn]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(30)]
        [Required(ErrorMessage = "Last name is required")]
        [EncryptColumn]
        public string LastName { get; set; }

        [EncryptColumn]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [MaxLength(30)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [EncryptColumn]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [EncryptColumn]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [EncryptColumn]
        public string DateOfBirth { get; set; }
        [EncryptColumn]
        public string Gender { get; set; }

        [MaxLength(300)]
        [Required(ErrorMessage = "Address is required")]
        [EncryptColumn]
        public string Address { get; set; }


        [EncryptColumn]
        [MaxLength(20)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Invalid city. Only alphabets are allowed")]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [EncryptColumn]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip is required")]
        [RegularExpression(@"^\d{5}(-\d{4})?", ErrorMessage = "Invalid Zip Code")]
        [EncryptColumn]
        public string Zip { get; set; }


        [EncryptColumn]
        [RegularExpression(@"^(\+1[-\s.]?)?(\(?\d{3}\)?[-\s.]?)?\d{3}[-\s.]?\d{4}$", ErrorMessage = "Invalid phone number")]
        [MaxLength(500)]
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }

        [MaxLength(500)]
        [EncryptColumn]
        public string LawFirmName { get; set; }
        [EncryptColumn]
        public string UserId { get; set; }
        [EncryptColumn]
        public string ParentUserId { get; set; }
        [Required(ErrorMessage = "Role is required")]
        [EncryptColumn]
        public string RoleName { get; set; }
        [EncryptColumn]
        public int UserCreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<UsaStatesViewModel> StateList { get; set; } = new List<UsaStatesViewModel>();
        public List<RolesViewModel> RoleList { get; set; } = new List<RolesViewModel>();
        public bool IsActive { get; set; }
        public List<LawFirmStaffVM> lawFirmStaffVMs { get; set; }
        public List<PateintDataRequestModel> PateintDataRequests { get; set; }


    }
}
