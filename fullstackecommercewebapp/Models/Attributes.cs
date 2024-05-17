using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace fullstackecommercewebapp.Models
{
    public partial class Attributes : BaseEntity
    {
        public Attributes()
        {
            CategoryAttribute = new HashSet<CategoryAttribute>();
            ProductAttributeValue = new HashSet<ProductAttributeValue>();
        }
       
        public int Id { get; set; }
        [Display(Name = "Attribute Name")]
        public string Name { get; set; }

        public virtual ICollection<CategoryAttribute>? CategoryAttribute { get; set; }
        public virtual ICollection<ProductAttributeValue>? ProductAttributeValue { get; set; }
    }
}
