using fullstackecommercewebapp.Models;
using System.ComponentModel.DataAnnotations;

namespace fullstackecommercewebapp.ViewModels
{
    public class values
    {
        public values()
        {
        }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            Categories = null;
            CategoryAttributes = null;
            NonCategoryAttributes = null;
            AddedAttributes = null;
            ProductAttributeValues = null;
            SelectedAttributes = new List<int>();
            AddedProductAttributeValue = new List<values>();
        }
        [Required(ErrorMessage = "Product Name must not be empty")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "SKU must not be empty")]
        [Display(Name = "Product SKU")]
        public string Sku { get; set; }

        [Required(ErrorMessage = "Price must not be empty")]
        [Display(Name = "Unit Price")]
        public double Price { get; set; }

        [Display(Name = "Product Image")]
        public string? Image1 { get; set; }

        public string Description { get; set; }

        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "Company Name must not be empty")]
        public string CompanyName { get; set; }

        public int CategoryId { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<Models.Attributes>? NonCategoryAttributes { get; set; }
        public IEnumerable<Models.Attributes>? CategoryAttributes { get; set; }
        public IEnumerable<Models.Attributes>? AddedAttributes { get; set; }
        public ICollection<int> SelectedAttributes { get; set; }
        public ICollection<values>? AddedProductAttributeValue { get; set; }
        public IEnumerable<ProductAttributeValue>? ProductAttributeValues { get; set; }

    }
}
