using System.ComponentModel.DataAnnotations;

namespace fullstackecommercewebapp.ViewModels
{
    public class AttributeViewModel
    {
        public AttributeViewModel()
        {
        }

        [Required(ErrorMessage = "Attribute Name must not be empty")]
        [Display(Name = "Attribute Name")]
        public string Name { get; set; }

    }
}
