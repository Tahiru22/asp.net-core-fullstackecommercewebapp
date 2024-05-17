using System.ComponentModel.DataAnnotations;

namespace fullstackecommercewebapp.ViewModels
{

    public class ForgotPasswordViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
