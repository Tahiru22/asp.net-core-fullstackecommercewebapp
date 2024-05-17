using fullstackecommercewebapp.Models;
using System.ComponentModel.DataAnnotations;

namespace fullstackecommercewebapp.ViewModels
{

    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
            SelectedAttributes = new List<int>();
            AddedAttributes = new List<Models.Attributes>();
            Attributes = null;
            CategoryAttributes = null;
            cat_products = null;
        }

        [Required(ErrorMessage = "Category Name must not be empty")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<Product>? cat_products { get; set; }
        public IEnumerable<Models.Attributes>? Attributes { get; set; }

        public List<int> SelectedAttributes { get; set; }

        public ICollection<Models.Attributes>? AddedAttributes { get; set; }


        public IEnumerable<Models.Attributes>? CategoryAttributes { get; set; } // referene to details of category


    }
}
