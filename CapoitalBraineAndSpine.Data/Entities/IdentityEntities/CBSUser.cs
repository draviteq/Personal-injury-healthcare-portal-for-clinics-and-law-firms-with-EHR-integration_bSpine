using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapitalBraineAndSpine.Data.Entities.IdentityEntities
{
    public class CBSUser : IdentityUser
    {
        [PersonalData]
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [EncryptColumn]
        public string FirstName { get; set; }

        [PersonalData]
        [EncryptColumn]
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        [PersonalData]
        [EncryptColumn]
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public override string Email { get; set; }

        [PersonalData]
        [EncryptColumn]
        [Column(TypeName = "nvarchar(200)")]
        public override string NormalizedEmail { get; set; }

        [PersonalData]
        [EncryptColumn]
        [Column(TypeName = "nvarchar(200)")]
        public override string UserName { get; set; }

        [PersonalData]
        [EncryptColumn]
        [Column(TypeName = "nvarchar(200)")]
        public override string NormalizedUserName { get; set; }

        [PersonalData]
        [EncryptColumn]
        [Column(TypeName = "nvarchar(200)")]
        public override string PhoneNumber { get; set; }

        public string ProfileImage { get; set; }
        public int PrefixVal { get; set; }

        [EncryptColumn]
        [Column(TypeName = "nvarchar(200)")]
        public string CustomUserId { get; set; }
        public bool IsActive { get; set; }
        public int? Otp { get; set; }
        public DateTime? OtpCreated { set; get; }
        public bool? OtpUsed { set; get; }
        [EncryptColumn]
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        [EncryptColumn]
        [Column(TypeName = "nvarchar(2000)")]
        public string Address { get; set; }

        [EncryptColumn]
        [Column(TypeName = "nvarchar(1000)")]
        public string City { get; set; }
        [EncryptColumn]
        [Column(TypeName = "nvarchar(1000)")]
        public string State { get; set; }
        [EncryptColumn]
        [Column(TypeName = "nvarchar(1000)")]
        public string Zip { get; set; }

        [EncryptColumn]
        [Column(TypeName = "nvarchar(1000)")]
        public string LawFirmName { get; set; }
        [EncryptColumn]
        [Column(TypeName = "nvarchar(1000)")]
        public string ParentUserId { get; set; }
        [EncryptColumn]
        [Column(TypeName = "nvarchar(1000)")]
        public string UserCreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
