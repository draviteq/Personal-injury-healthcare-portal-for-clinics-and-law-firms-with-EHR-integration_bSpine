using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CapitalBraineAndSpine.Data.Entities.IdentityEntities
{
    public class CBSRole : IdentityRole
    {
        [Required]
        [MaxLength(256)]
        public string Caption { get; set; }

        [MaxLength(10)]
        public string Prefix { get; set; }


        [MaxLength(20)]
        public string DisplayName { get; set; }
    }
}
