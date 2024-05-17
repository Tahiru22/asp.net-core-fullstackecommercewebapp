namespace fullstackecommercewebapp.Models
{

    public partial class ProductOrder : BaseEntity
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
