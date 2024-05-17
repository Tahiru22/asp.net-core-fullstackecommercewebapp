using System.ComponentModel.DataAnnotations;

namespace fullstackecommercewebapp.ViewModels
{
    public class MyAccountViewModel
    {
        public MyAccountViewModel()
        {
        }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Gender")]
        public string Gendre { get; set; }

        [Required]
        [RegularExpression(@"(^233[0-9]{8}$)", ErrorMessage = "Phone Number must be a Ghanaian Mobile Phone Number (10 digits and the first two are 233)")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Display(Name = "Personal Photo")]
        public string? Image { get; set; }
    }
}
