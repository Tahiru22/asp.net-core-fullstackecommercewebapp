using fullstackecommercewebapp.Models;
using System.ComponentModel.DataAnnotations;

namespace fullstackecommercewebapp.ViewModels
{
    public class UserViewModelEdit
    {
        public UserViewModelEdit()
        {
            rolesIds = new List<int>();
            roles = new List<AspNetRoles>();
            rolesNames = new List<string>();
        }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "User First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "User Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "User Address")]
        public string Address { get; set; }

        [Display(Name = "User Gender")]
        public string Gendre { get; set; }

        [Required]
        [RegularExpression(@"(^233[0-9]{8}$)", ErrorMessage = "Phone Number must be a Ghanaian Mobile Phone Number (10 digits and the first three are 233)")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Display(Name = "Personal Photo")]
        public string? Image { get; set; }
        public List<int> rolesIds { get; set; }
        public List<AspNetRoles> roles { get; set; }
        public List<string> rolesNames { get; set; }
    }
}
