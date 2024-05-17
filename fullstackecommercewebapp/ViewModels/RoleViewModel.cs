using System.ComponentModel.DataAnnotations;

namespace fullstackecommercewebapp.ViewModels
{
    public class RoleViewModel
    {
        public RoleViewModel()
        {
        }

        [Required(ErrorMessage = "Role Name must not be empty")]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}
