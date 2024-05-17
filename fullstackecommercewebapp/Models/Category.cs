using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace fullstackecommercewebapp.Models
{
    public partial class Category : BaseEntity
    {
        public Category()
        {
            CategoryAttribute = new HashSet<CategoryAttribute>();
            Product = new HashSet<Product>();
        }

        public int Id { get; set; }
        [Display(Name = "Category Name")]
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<CategoryAttribute> CategoryAttribute { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
