namespace fullstackecommercewebapp.Models
{
    public partial class CartItem : BaseEntity
    {
        public int ProductId { get; set; }
        public int? CartId { get; set; }
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public virtual Product Product { get; set; }
    }
}
