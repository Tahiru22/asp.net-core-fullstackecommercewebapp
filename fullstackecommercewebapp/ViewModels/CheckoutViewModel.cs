using System.ComponentModel.DataAnnotations;

namespace fullstackecommercewebapp.ViewModels
{
    public class CheckoutViewModel
    {
        public CheckoutViewModel()
        {
            total = 0.0;
        }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [RegularExpression(@"(^233[0-9]{8}$)", ErrorMessage = "Phone Number must be a Ghanaian Mobile Phone Number (10 digits and the first three are 233)")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        public double total { get; set; }

    }
}
