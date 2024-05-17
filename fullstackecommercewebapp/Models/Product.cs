using System.ComponentModel.DataAnnotations;

namespace fullstackecommercewebapp.Models
{
    public partial class Product : BaseEntity
    {
        public Product()
        {
            CouponProduct = new HashSet<CouponProduct>();
            ProductAttributeValue = new HashSet<ProductAttributeValue>();
            ProductOrder = new HashSet<ProductOrder>();
            CartItems = new HashSet<CartItem>();
            Reviews = new HashSet<Reviews>();
            SaleProduct = new HashSet<SaleProduct>();
            WishList = new HashSet<WishList>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string? Image1 { get; set; }
        public string Description { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<CouponProduct>? CouponProduct { get; set; }
        public virtual ICollection<ProductAttributeValue>? ProductAttributeValue { get; set; }
        public virtual ICollection<ProductOrder>? ProductOrder { get; set; }
        public virtual ICollection<CartItem>? CartItems { get; set; }
        public virtual ICollection<Reviews>? Reviews { get; set; }
        public virtual ICollection<SaleProduct>? SaleProduct { get; set; }
        public virtual ICollection<WishList>? WishList { get; set; }
    }
}
