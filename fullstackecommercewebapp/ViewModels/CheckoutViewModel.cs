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
        [Required]
        public string Address { get; set; }

        //[RegularExpression(@"(^\+[1-9]\d{1,14}$)", ErrorMessage = "Phone Number must be in E164 format")]
        [Display(Name = "Phone Number")]
        [Required]
        public string Phone { get; set; }

        [Required]
        public double total { get; set; }

        [Display(Name = "Reference")]
        [Required]
        public string PaystackPaymentReference { get; set; }

    }
}
