namespace fullstackecommercewebapp.Models
{
    public partial class Order : BaseEntity
    {
        public Order()
        {
            ProductOrder = new HashSet<ProductOrder>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Status { get; set; }
        public double Total { get; set; }

        public virtual User Customer { get; set; }
        public virtual Shipping Shipping { get; set; }
        public virtual ICollection<ProductOrder> ProductOrder { get; set; }
    }
}
